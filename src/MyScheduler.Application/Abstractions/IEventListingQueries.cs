using MyScheduler.Application.Contracts;

namespace MyScheduler.Application.Abstractions;

public interface IEventListingQueries
{
    Task<EventDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<PagedResult<EventSummaryDto>> ListAsync(EventListFilter filter, CancellationToken cancellationToken);

    Task<PagedResult<EventSummaryDto>> SearchAsync(string searchTerm, int page, int pageSize, CancellationToken cancellationToken);

    /// <summary>True if the attendee has no Scheduled event overlapping the given range.</summary>
    Task<bool> IsAttendeeAvailableAsync(Guid attendeeId, DateTime startTimeUtc, DateTime endTimeUtc, CancellationToken cancellationToken);
}
