using MyScheduler.Domain.Notifications;
using MyScheduler.Infrastructure.Outbox;
using MyScheduler.Infrastructure.UnitTests.Fakes;

namespace MyScheduler.Infrastructure.UnitTests.Outbox;

public class NotificationDispatcherTests
{
    private static readonly DateTime NowUtc = new(2026, 1, 1, 9, 0, 0, DateTimeKind.Utc);

    private static Notification CreateNotification(NotificationChannel channel) => Notification.Create(
        Guid.NewGuid(), Guid.NewGuid(), channel, NotificationTriggerType.EventCreated, "{}", NowUtc);

    [Fact]
    public async Task DispatchPendingBatchAsync_PassesBatchSizeThrough_ToOutboxRepository()
    {
        var outbox = new FakeNotificationOutboxRepository();
        var dispatcher = new NotificationDispatcher(outbox, [], new FakeUnitOfWork(), new FakeDateTimeProvider(NowUtc));

        await dispatcher.DispatchPendingBatchAsync(batchSize: 7, CancellationToken.None);

        Assert.Equal(7, outbox.LastRequestedBatchSize);
    }

    [Fact]
    public async Task DispatchPendingBatchAsync_RoutesToTheSenderThatCanHandleTheChannel()
    {
        var notification = CreateNotification(NotificationChannel.Email);
        var outbox = new FakeNotificationOutboxRepository();
        outbox.Notifications.Add(notification);

        var emailSender = new FakeNotificationChannelSender(NotificationChannel.Email);
        var icalSender = new FakeNotificationChannelSender(NotificationChannel.ICal);
        var dispatcher = new NotificationDispatcher(
            outbox, [icalSender, emailSender], new FakeUnitOfWork(), new FakeDateTimeProvider(NowUtc));

        await dispatcher.DispatchPendingBatchAsync(10, CancellationToken.None);

        Assert.Contains(notification, emailSender.Sent);
        Assert.Empty(icalSender.Sent);
    }

    [Fact]
    public async Task DispatchPendingBatchAsync_WhenSendSucceeds_MarksSentAndSaves()
    {
        var notification = CreateNotification(NotificationChannel.Email);
        var outbox = new FakeNotificationOutboxRepository();
        outbox.Notifications.Add(notification);
        var unitOfWork = new FakeUnitOfWork();

        var sender = new FakeNotificationChannelSender(NotificationChannel.Email);
        var dispatcher = new NotificationDispatcher(outbox, [sender], unitOfWork, new FakeDateTimeProvider(NowUtc));

        await dispatcher.DispatchPendingBatchAsync(10, CancellationToken.None);

        Assert.Equal(NotificationStatus.Sent, notification.Status);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task DispatchPendingBatchAsync_WhenNoSenderRegisteredForChannel_MarksFailed()
    {
        var notification = CreateNotification(NotificationChannel.Mq);
        var outbox = new FakeNotificationOutboxRepository();
        outbox.Notifications.Add(notification);

        var emailSender = new FakeNotificationChannelSender(NotificationChannel.Email);
        var dispatcher = new NotificationDispatcher(
            outbox, [emailSender], new FakeUnitOfWork(), new FakeDateTimeProvider(NowUtc));

        await dispatcher.DispatchPendingBatchAsync(10, CancellationToken.None);

        Assert.Equal(NotificationStatus.Pending, notification.Status);
        Assert.Equal(1, notification.Attempts);
        Assert.Contains("No sender registered", notification.LastErrorMessage);
    }

    [Fact]
    public async Task DispatchPendingBatchAsync_WhenSenderThrows_MarksFailedWithExceptionMessage()
    {
        var notification = CreateNotification(NotificationChannel.Email);
        var outbox = new FakeNotificationOutboxRepository();
        outbox.Notifications.Add(notification);

        var sender = new FakeNotificationChannelSender(NotificationChannel.Email)
        {
            ThrowOnSend = new InvalidOperationException("smtp unreachable"),
        };
        var dispatcher = new NotificationDispatcher(outbox, [sender], new FakeUnitOfWork(), new FakeDateTimeProvider(NowUtc));

        await dispatcher.DispatchPendingBatchAsync(10, CancellationToken.None);

        Assert.Equal("smtp unreachable", notification.LastErrorMessage);
    }

    [Fact]
    public async Task DispatchPendingBatchAsync_WhenBatchEmpty_DoesNotCallSaveChanges()
    {
        var unitOfWork = new FakeUnitOfWork();
        var dispatcher = new NotificationDispatcher(
            new FakeNotificationOutboxRepository(), [], unitOfWork, new FakeDateTimeProvider(NowUtc));

        await dispatcher.DispatchPendingBatchAsync(10, CancellationToken.None);

        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task DispatchPendingBatchAsync_AfterFiveFailedAttemptsAcrossPolls_BecomesPermanentlyFailed()
    {
        var notification = CreateNotification(NotificationChannel.Email);
        var outbox = new FakeNotificationOutboxRepository();
        outbox.Notifications.Add(notification);

        var sender = new FakeNotificationChannelSender(NotificationChannel.Email)
        {
            ThrowOnSend = new InvalidOperationException("smtp unreachable"),
        };
        var dispatcher = new NotificationDispatcher(outbox, [sender], new FakeUnitOfWork(), new FakeDateTimeProvider(NowUtc));

        // Each poll tick only re-processes still-Pending notifications, mirroring how the
        // background service would retry across successive ticks.
        for (var i = 0; i < 4; i++)
        {
            await dispatcher.DispatchPendingBatchAsync(10, CancellationToken.None);
            Assert.Equal(NotificationStatus.Pending, notification.Status);
        }

        await dispatcher.DispatchPendingBatchAsync(10, CancellationToken.None);

        Assert.Equal(NotificationStatus.Failed, notification.Status);
        Assert.Equal(5, notification.Attempts);
    }
}
