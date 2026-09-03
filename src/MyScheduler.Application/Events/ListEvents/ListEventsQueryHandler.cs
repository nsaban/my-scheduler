using MediatR;
using MyScheduler.Application.Abstractions;
using MyScheduler.Application.Contracts;

namespace MyScheduler.Application.Events.ListEvents;

public sealed class ListEventsQueryHandler(IEventListingQueries eventListingQueries)
    : IRequestHandler<ListEventsQuery, PagedResult<EventSummaryDto>>
{
    public Task<PagedResult<EventSummaryDto>> Handle(ListEventsQuery request, CancellationToken cancellationToken)
    {
        var filter = new EventListFilter
        {
            FromUtc = request.FromUtc,
            ToUtc = request.ToUtc,
            Status = request.Status,
            OrganizerId = request.OrganizerId,
            AttendeeId = request.AttendeeId,
            Page = request.Page,
            PageSize = request.PageSize,
        };

        return eventListingQueries.ListAsync(filter, cancellationToken);
    }
}
