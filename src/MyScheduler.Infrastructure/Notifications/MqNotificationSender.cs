using Microsoft.Extensions.Logging;
using MyScheduler.Application.Abstractions;
using MyScheduler.Domain.Notifications;

namespace MyScheduler.Infrastructure.Notifications;

public sealed class MqNotificationSender(ILogger<MqNotificationSender> logger) : INotificationChannelSender
{
    public bool CanHandle(NotificationChannel channel) => channel == NotificationChannel.Mq;

    public Task SendAsync(Notification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Would publish notification {NotificationId} to the message queue (attendee {RecipientAttendeeId}, event {EventId})",
            notification.Id, notification.RecipientAttendeeId, notification.EventId);

        return Task.CompletedTask;
    }
}
