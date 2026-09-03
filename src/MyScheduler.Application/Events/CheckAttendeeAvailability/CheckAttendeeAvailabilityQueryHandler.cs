using MediatR;
using MyScheduler.Application.Abstractions;

namespace MyScheduler.Application.Events.CheckAttendeeAvailability;

public sealed class CheckAttendeeAvailabilityQueryHandler(IEventListingQueries eventListingQueries)
    : IRequestHandler<CheckAttendeeAvailabilityQuery, bool>
{
    public Task<bool> Handle(CheckAttendeeAvailabilityQuery request, CancellationToken cancellationToken) =>
        eventListingQueries.IsAttendeeAvailableAsync(request.AttendeeId, request.StartTimeUtc, request.EndTimeUtc, cancellationToken);
}
