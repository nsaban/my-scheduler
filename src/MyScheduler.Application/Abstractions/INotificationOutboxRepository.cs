using MyScheduler.Domain.Notifications;

namespace MyScheduler.Application.Abstractions;

public interface INotificationOutboxRepository
{
    Task AddAsync(Notification notification, CancellationToken cancellationToken);

    /// <summary>
    /// Claims up to <paramref name="batchSize"/> pending notifications for processing, skipping
    /// rows already claimed by another concurrent dispatcher instance.
    /// </summary>
    Task<IReadOnlyList<Notification>> GetPendingBatchAsync(int batchSize, CancellationToken cancellationToken);
}
