using Microsoft.Extensions.Logging;
using MyScheduler.Application.Abstractions;
using MyScheduler.Domain.Notifications;

namespace MyScheduler.Infrastructure.Notifications;

public sealed class EmailNotificationSender(ILogger<EmailNotificationSender> logger) : INotificationChannelSender
{
    public bool CanHandle(NotificationChannel channel) => channel == NotificationChannel.Email;

    public Task SendAsync(Notification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Would send Email notification {NotificationId} to attendee {RecipientAttendeeId} for event {EventId}: {Payload}",
            notification.Id, notification.RecipientAttendeeId, notification.EventId, notification.Payload);

        return Task.CompletedTask;
    }
}
