using MyScheduler.Application.Abstractions;
using MyScheduler.Application.Contracts;

namespace MyScheduler.Application.UnitTests.Fakes;

public sealed class FakeEventListingQueries : IEventListingQueries
{
    public EventDto? EventToReturn { get; set; }

    public PagedResult<EventSummaryDto> ListResult { get; set; } = new();

    public PagedResult<EventSummaryDto> SearchResult { get; set; } = new();

    public bool IsAvailableResult { get; set; } = true;

    public EventListFilter? LastFilter { get; private set; }

    public string? LastSearchTerm { get; private set; }

    public Task<EventDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken) => Task.FromResult(EventToReturn);

    public Task<PagedResult<EventSummaryDto>> ListAsync(EventListFilter filter, CancellationToken cancellationToken)
    {
        LastFilter = filter;
        return Task.FromResult(ListResult);
    }

    public Task<PagedResult<EventSummaryDto>> SearchAsync(string searchTerm, int page, int pageSize, CancellationToken cancellationToken)
    {
        LastSearchTerm = searchTerm;
        return Task.FromResult(SearchResult);
    }

    public Task<bool> IsAttendeeAvailableAsync(Guid attendeeId, DateTime startTimeUtc, DateTime endTimeUtc, CancellationToken cancellationToken) =>
        Task.FromResult(IsAvailableResult);
}
