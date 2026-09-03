using MyScheduler.Domain.Attendees;

namespace MyScheduler.Application.Abstractions;

public interface IAttendeeRepository
{
    Task<Attendee?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Attendee>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);

    Task AddAsync(Attendee attendee, CancellationToken cancellationToken);
}
