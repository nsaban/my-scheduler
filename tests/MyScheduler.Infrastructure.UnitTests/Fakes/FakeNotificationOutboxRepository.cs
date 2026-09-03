using MyScheduler.Application.Abstractions;
using MyScheduler.Domain.Notifications;

namespace MyScheduler.Infrastructure.UnitTests.Fakes;

public sealed class FakeNotificationOutboxRepository : INotificationOutboxRepository
{
    public List<Notification> Notifications { get; } = [];

    public int? LastRequestedBatchSize { get; private set; }

    public Task AddAsync(Notification notification, CancellationToken cancellationToken)
    {
        Notifications.Add(notification);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Notification>> GetPendingBatchAsync(int batchSize, CancellationToken cancellationToken)
    {
        LastRequestedBatchSize = batchSize;

        var batch = Notifications.Where(n => n.Status == NotificationStatus.Pending).Take(batchSize).ToList();
        return Task.FromResult<IReadOnlyList<Notification>>(batch);
    }
}
