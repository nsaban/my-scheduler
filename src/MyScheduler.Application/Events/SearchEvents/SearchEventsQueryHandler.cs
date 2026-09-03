using MediatR;
using MyScheduler.Application.Abstractions;
using MyScheduler.Application.Contracts;

namespace MyScheduler.Application.Events.SearchEvents;

public sealed class SearchEventsQueryHandler(IEventListingQueries eventListingQueries)
    : IRequestHandler<SearchEventsQuery, PagedResult<EventSummaryDto>>
{
    public Task<PagedResult<EventSummaryDto>> Handle(SearchEventsQuery request, CancellationToken cancellationToken) =>
        eventListingQueries.SearchAsync(request.SearchTerm, request.Page, request.PageSize, cancellationToken);
}
