using MyScheduler.Domain.Notifications;

namespace MyScheduler.Application.Abstractions;

public interface INotificationChannelSender
{
    bool CanHandle(NotificationChannel channel);

    Task SendAsync(Notification notification, CancellationToken cancellationToken);
}
