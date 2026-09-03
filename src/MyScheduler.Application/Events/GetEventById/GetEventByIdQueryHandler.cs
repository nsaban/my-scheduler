using MediatR;
using MyScheduler.Application.Abstractions;
using MyScheduler.Application.Contracts;

namespace MyScheduler.Application.Events.GetEventById;

public sealed class GetEventByIdQueryHandler(IEventListingQueries eventListingQueries)
    : IRequestHandler<GetEventByIdQuery, EventDto?>
{
    public Task<EventDto?> Handle(GetEventByIdQuery request, CancellationToken cancellationToken) =>
        eventListingQueries.GetByIdAsync(request.EventId, cancellationToken);
}
