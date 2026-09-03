using Microsoft.EntityFrameworkCore;
using MyScheduler.Application.Abstractions;
using MyScheduler.Domain.Attendees;

namespace MyScheduler.Persistence.Repositories;

public sealed class AttendeeRepository(AppDbContext dbContext) : IAttendeeRepository
{
    public Task<Attendee?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Attendees.SingleOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Attendee>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        var idList = ids.Distinct().ToList();

        return await dbContext.Attendees
            .Where(a => idList.Contains(a.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Attendee attendee, CancellationToken cancellationToken) =>
        await dbContext.Attendees.AddAsync(attendee, cancellationToken);
}
