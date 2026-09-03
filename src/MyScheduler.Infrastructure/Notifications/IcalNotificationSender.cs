using Microsoft.Extensions.Logging;
using MyScheduler.Application.Abstractions;
using MyScheduler.Domain.Notifications;

namespace MyScheduler.Infrastructure.Notifications;

public sealed class IcalNotificationSender(ILogger<IcalNotificationSender> logger) : INotificationChannelSender
{
    public bool CanHandle(NotificationChannel channel) => channel == NotificationChannel.ICal;

    public Task SendAsync(Notification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Would generate an iCal invite for notification {NotificationId} (attendee {RecipientAttendeeId}, event {EventId})",
            notification.Id, notification.RecipientAttendeeId, notification.EventId);

        return Task.CompletedTask;
    }
}
