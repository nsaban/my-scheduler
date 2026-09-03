using MyScheduler.Application.Abstractions;
using MyScheduler.Domain.Attendees;

namespace MyScheduler.Application.UnitTests.Fakes;

public sealed class FakeAttendeeRepository : IAttendeeRepository
{
    public Dictionary<Guid, Attendee> Attendees { get; } = [];

    public Task<Attendee?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(Attendees.GetValueOrDefault(id));

    public Task<IReadOnlyList<Attendee>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        var result = ids.Where(Attendees.ContainsKey).Select(id => Attendees[id]).ToList();
        return Task.FromResult<IReadOnlyList<Attendee>>(result);
    }

    public Task AddAsync(Attendee attendee, CancellationToken cancellationToken)
    {
        Attendees[attendee.Id] = attendee;
        return Task.CompletedTask;
    }
}
