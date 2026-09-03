using Microsoft.EntityFrameworkCore;
using MyScheduler.Application.Abstractions;
using MyScheduler.Domain.Notifications;

namespace MyScheduler.Persistence.Outbox;

public sealed class NotificationOutboxRepository(AppDbContext dbContext) : INotificationOutboxRepository
{
    public async Task AddAsync(Notification notification, CancellationToken cancellationToken) =>
        await dbContext.Notifications.AddAsync(notification, cancellationToken);

    public async Task<IReadOnlyList<Notification>> GetPendingBatchAsync(int batchSize, CancellationToken cancellationToken)
    {
        var pendingStatus = (int)NotificationStatus.Pending;

        // UPDLOCK+ROWLOCK+READPAST: claim a batch while skipping rows another dispatcher instance
        // already has locked, so two workers never double-send the same notification.
        return await dbContext.Notifications
            .FromSqlInterpolated($"""
                SELECT TOP ({batchSize}) *
                FROM Notifications WITH (UPDLOCK, ROWLOCK, READPAST)
                WHERE Status = {pendingStatus}
                ORDER BY CreatedAtUtc
                """)
            .ToListAsync(cancellationToken);
    }
}
