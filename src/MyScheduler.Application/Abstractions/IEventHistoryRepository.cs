using MyScheduler.Domain.History;

namespace MyScheduler.Application.Abstractions;

public interface IEventHistoryRepository
{
    Task<int> GetNextVersionAsync(Guid eventId, CancellationToken cancellationToken);

    Task AddAsync(EventHistory eventHistory, CancellationToken cancellationToken);
}
