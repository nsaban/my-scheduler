using Microsoft.Extensions.Logging.Abstractions;
using MyScheduler.Application.Abstractions;
using MyScheduler.Domain.Notifications;
using MyScheduler.Infrastructure.Notifications;

namespace MyScheduler.Infrastructure.UnitTests.Notifications;

public class NotificationChannelSenderTests
{
    private static readonly DateTime NowUtc = new(2026, 1, 1, 9, 0, 0, DateTimeKind.Utc);

    public static TheoryData<INotificationChannelSender, NotificationChannel> Senders() => new()
    {
        { new EmailNotificationSender(NullLogger<EmailNotificationSender>.Instance), NotificationChannel.Email },
        { new IcalNotificationSender(NullLogger<IcalNotificationSender>.Instance), NotificationChannel.ICal },
        { new MqNotificationSender(NullLogger<MqNotificationSender>.Instance), NotificationChannel.Mq },
    };

    [Theory]
    [MemberData(nameof(Senders))]
    public void CanHandle_OnlyMatchesItsOwnChannel(INotificationChannelSender sender, NotificationChannel ownChannel)
    {
        foreach (var channel in Enum.GetValues<NotificationChannel>())
        {
            Assert.Equal(channel == ownChannel, sender.CanHandle(channel));
        }
    }

    [Theory]
    [MemberData(nameof(Senders))]
    public async Task SendAsync_DoesNotThrow(INotificationChannelSender sender, NotificationChannel channel)
    {
        var notification = Notification.Create(
            Guid.NewGuid(), Guid.NewGuid(), channel, NotificationTriggerType.EventCreated, "{}", NowUtc);

        await sender.SendAsync(notification, CancellationToken.None);
    }
}
