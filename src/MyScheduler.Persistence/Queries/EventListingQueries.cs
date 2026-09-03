using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MyScheduler.Application.Abstractions;
using MyScheduler.Application.Contracts;
using MyScheduler.Domain.Scheduling;

namespace MyScheduler.Persistence.Queries;

public sealed class EventListingQueries(AppDbContext dbContext, IMapper mapper) : IEventListingQueries
{
    public async Task<EventDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var @event = await dbContext.Events
            .Include(e => e.EventAttendees)
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (@event is null)
        {
            return null;
        }

        var dto = mapper.Map<EventDto>(@event);
        var rowVersion = dbContext.Entry(@event).Property<byte[]>("RowVersion").CurrentValue ?? [];

        return dto with { RowVersion = rowVersion };
    }

    public Task<PagedResult<EventSummaryDto>> ListAsync(EventListFilter filter, CancellationToken cancellationToken)
    {
        IQueryable<Event> query = dbContext.Events;

        if (filter.FromUtc.HasValue)
        {
            query = query.Where(e => e.TimeRange.End > filter.FromUtc.Value);
        }

        if (filter.ToUtc.HasValue)
        {
            query = query.Where(e => e.TimeRange.Start < filter.ToUtc.Value);
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(e => e.Status == filter.Status.Value);
        }

        if (filter.OrganizerId.HasValue)
        {
            query = query.Where(e => e.OrganizerId == filter.OrganizerId.Value);
        }

        if (filter.AttendeeId.HasValue)
        {
            query = query.Where(e => e.EventAttendees.Any(ea => ea.AttendeeId == filter.AttendeeId.Value));
        }

        return PaginateAsync(query, filter.Page, filter.PageSize, cancellationToken);
    }

    public Task<PagedResult<EventSummaryDto>> SearchAsync(string searchTerm, int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = dbContext.Events.Where(e =>
            EF.Functions.Contains(e.Title, searchTerm) || EF.Functions.Contains(e.Description!, searchTerm));

        return PaginateAsync(query, page, pageSize, cancellationToken);
    }

    public async Task<bool> IsAttendeeAvailableAsync(Guid attendeeId, DateTime startTimeUtc, DateTime endTimeUtc, CancellationToken cancellationToken)
    {
        var hasConflict = await dbContext.Events
            .Where(e => e.Status == EventStatus.Scheduled)
            .Where(e => e.EventAttendees.Any(ea => ea.AttendeeId == attendeeId))
            .Where(e => e.TimeRange.Start < endTimeUtc && startTimeUtc < e.TimeRange.End)
            .AnyAsync(cancellationToken);

        return !hasConflict;
    }

    private async Task<PagedResult<EventSummaryDto>> PaginateAsync(
        IQueryable<Event> query, int page, int pageSize, CancellationToken cancellationToken)
    {
        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(e => e.TimeRange.Start)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectTo<EventSummaryDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new PagedResult<EventSummaryDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
        };
    }
}
