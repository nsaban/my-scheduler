using MyScheduler.Application.Abstractions;
using MyScheduler.Domain.Scheduling;

namespace MyScheduler.Application.UnitTests.Fakes;

public sealed class FakeEventRepository : IEventRepository
{
    public Dictionary<Guid, Event> Events { get; } = [];

    public Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(Events.GetValueOrDefault(id));

    public Task<Event?> GetForUpdateAsync(Guid id, byte[] expectedVersion, CancellationToken cancellationToken) =>
        Task.FromResult(Events.GetValueOrDefault(id));

    public Task AddAsync(Event @event, CancellationToken cancellationToken)
    {
        Events[@event.Id] = @event;
        return Task.CompletedTask;
    }
}
