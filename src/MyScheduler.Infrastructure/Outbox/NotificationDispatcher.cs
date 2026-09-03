using MyScheduler.Application.Abstractions;

namespace MyScheduler.Infrastructure.Outbox;

public sealed class NotificationDispatcher(
    INotificationOutboxRepository outboxRepository,
    IEnumerable<INotificationChannelSender> senders,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider)
{
    public async Task<int> DispatchPendingBatchAsync(int batchSize, CancellationToken cancellationToken)
    {
        var batch = await outboxRepository.GetPendingBatchAsync(batchSize, cancellationToken);

        foreach (var notification in batch)
        {
            var sender = senders.FirstOrDefault(s => s.CanHandle(notification.Channel));

            if (sender is null)
            {
                notification.MarkFailed($"No sender registered for channel '{notification.Channel}'.");
                continue;
            }

            try
            {
                await sender.SendAsync(notification, cancellationToken);
                notification.MarkSent(dateTimeProvider.UtcNow);
            }
            catch (Exception ex)
            {
                notification.MarkFailed(ex.Message);
            }
        }

        if (batch.Count > 0)
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return batch.Count;
    }
}
