using Microsoft.EntityFrameworkCore;
using MyScheduler.Application.Abstractions;
using MyScheduler.Domain.Scheduling;

namespace MyScheduler.Persistence.Repositories;

public sealed class EventRepository(AppDbContext dbContext) : IEventRepository
{
    public Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Events
            .Include(e => e.EventAttendees)
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<Event?> GetForUpdateAsync(Guid id, byte[] expectedVersion, CancellationToken cancellationToken)
    {
        var @event = await dbContext.Events
            .Include(e => e.EventAttendees)
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (@event is not null)
        {
            dbContext.Entry(@event).Property("RowVersion").OriginalValue = expectedVersion;
        }

        return @event;
    }

    public async Task AddAsync(Event @event, CancellationToken cancellationToken) =>
        await dbContext.Events.AddAsync(@event, cancellationToken);
}
