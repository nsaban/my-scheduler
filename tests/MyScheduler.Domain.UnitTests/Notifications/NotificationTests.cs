using MyScheduler.Domain.Common;
using MyScheduler.Domain.Notifications;

namespace MyScheduler.Domain.UnitTests.Notifications;

public class NotificationTests
{
    private static readonly DateTime NowUtc = new(2026, 1, 1, 9, 0, 0, DateTimeKind.Utc);

    private static Notification CreateNotification() => Notification.Create(
        Guid.NewGuid(),
        Guid.NewGuid(),
        NotificationChannel.Email,
        NotificationTriggerType.EventCreated,
        "{}",
        NowUtc);

    [Fact]
    public void Create_WhenValid_StartsPendingWithZeroAttempts()
    {
        var notification = CreateNotification();

        Assert.Equal(NotificationStatus.Pending, notification.Status);
        Assert.Equal(0, notification.Attempts);
        Assert.Null(notification.SentAtUtc);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WhenPayloadMissing_Throws(string payload)
    {
        Assert.Throws<DomainException>(() => Notification.Create(
            Guid.NewGuid(), Guid.NewGuid(), NotificationChannel.Email, NotificationTriggerType.EventCreated, payload, NowUtc));
    }

    [Fact]
    public void MarkSent_WhenPending_SetsSentStateAndClearsError()
    {
        var notification = CreateNotification();
        notification.MarkFailed("smtp timeout");

        notification.MarkSent(NowUtc.AddMinutes(1));

        Assert.Equal(NotificationStatus.Sent, notification.Status);
        Assert.Equal(NowUtc.AddMinutes(1), notification.SentAtUtc);
        Assert.Null(notification.LastErrorMessage);
        Assert.Equal(2, notification.Attempts);
    }

    [Fact]
    public void MarkSent_WhenAlreadySent_Throws()
    {
        var notification = CreateNotification();
        notification.MarkSent(NowUtc);

        Assert.Throws<DomainException>(() => notification.MarkSent(NowUtc));
    }

    [Fact]
    public void MarkFailed_BelowMaxAttempts_StaysPendingForRetry()
    {
        var notification = CreateNotification();

        notification.MarkFailed("smtp timeout");

        Assert.Equal(NotificationStatus.Pending, notification.Status);
        Assert.Equal(1, notification.Attempts);
        Assert.Equal("smtp timeout", notification.LastErrorMessage);
    }

    [Fact]
    public void MarkFailed_AtMaxAttempts_BecomesPermanentlyFailed()
    {
        var notification = CreateNotification();

        for (var i = 0; i < 4; i++)
        {
            notification.MarkFailed("smtp timeout");
        }

        Assert.Equal(NotificationStatus.Pending, notification.Status);

        notification.MarkFailed("smtp timeout");

        Assert.Equal(NotificationStatus.Failed, notification.Status);
        Assert.Equal(5, notification.Attempts);
    }

    [Fact]
    public void MarkFailed_TruncatesErrorMessageToMaxLength()
    {
        var notification = CreateNotification();
        var tooLong = new string('x', Notification.MaxLastErrorMessageLength + 100);

        notification.MarkFailed(tooLong);

        Assert.Equal(Notification.MaxLastErrorMessageLength, notification.LastErrorMessage!.Length);
    }

    [Fact]
    public void MarkFailed_WhenAlreadySent_Throws()
    {
        var notification = CreateNotification();
        notification.MarkSent(NowUtc);

        Assert.Throws<DomainException>(() => notification.MarkFailed("too late"));
    }
}
