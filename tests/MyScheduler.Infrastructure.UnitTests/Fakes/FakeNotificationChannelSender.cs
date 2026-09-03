using MyScheduler.Application.Abstractions;
using MyScheduler.Domain.Notifications;

namespace MyScheduler.Infrastructure.UnitTests.Fakes;

public sealed class FakeNotificationChannelSender(NotificationChannel channel) : INotificationChannelSender
{
    public List<Notification> Sent { get; } = [];

    public Exception? ThrowOnSend { get; set; }

    public bool CanHandle(NotificationChannel candidate) => candidate == channel;

    public Task SendAsync(Notification notification, CancellationToken cancellationToken)
    {
        if (ThrowOnSend is not null)
        {
            throw ThrowOnSend;
        }

        Sent.Add(notification);
        return Task.CompletedTask;
    }
}
