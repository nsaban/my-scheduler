using Microsoft.EntityFrameworkCore;
using MyScheduler.Application.Abstractions;
using MyScheduler.Domain.History;

namespace MyScheduler.Persistence.Repositories;

public sealed class EventHistoryRepository(AppDbContext dbContext) : IEventHistoryRepository
{
    public async Task<int> GetNextVersionAsync(Guid eventId, CancellationToken cancellationToken)
    {
        var currentMax = await dbContext.EventHistory
            .Where(h => h.EventId == eventId)
            .Select(h => (int?)h.Version)
            .MaxAsync(cancellationToken);

        return (currentMax ?? 0) + 1;
    }

    public async Task AddAsync(EventHistory eventHistory, CancellationToken cancellationToken) =>
        await dbContext.EventHistory.AddAsync(eventHistory, cancellationToken);
}
