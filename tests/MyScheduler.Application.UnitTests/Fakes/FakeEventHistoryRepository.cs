using MyScheduler.Application.Abstractions;
using MyScheduler.Domain.History;

namespace MyScheduler.Application.UnitTests.Fakes;

public sealed class FakeEventHistoryRepository : IEventHistoryRepository
{
    public List<EventHistory> Records { get; } = [];

    public Task<int> GetNextVersionAsync(Guid eventId, CancellationToken cancellationToken)
    {
        var max = Records.Where(h => h.EventId == eventId).Select(h => (int?)h.Version).Max();
        return Task.FromResult((max ?? 0) + 1);
    }

    public Task AddAsync(EventHistory eventHistory, CancellationToken cancellationToken)
    {
        Records.Add(eventHistory);
        return Task.CompletedTask;
    }
}
