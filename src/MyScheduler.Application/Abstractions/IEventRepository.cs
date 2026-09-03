using MyScheduler.Domain.Scheduling;

namespace MyScheduler.Application.Abstractions;

public interface IEventRepository
{
    Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Loads the event for a mutation guarded by optimistic concurrency: the returned
    /// aggregate's concurrency token is primed with <paramref name="expectedVersion"/>, so a
    /// stale caller's save fails even though this call itself always reads current data.
    /// </summary>
    Task<Event?> GetForUpdateAsync(Guid id, byte[] expectedVersion, CancellationToken cancellationToken);

    Task AddAsync(Event @event, CancellationToken cancellationToken);
}
