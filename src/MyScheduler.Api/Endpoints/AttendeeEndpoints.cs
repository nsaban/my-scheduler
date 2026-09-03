using MediatR;
using MyScheduler.Application.Events.CheckAttendeeAvailability;

namespace MyScheduler.Api.Endpoints;

public static class AttendeeEndpoints
{
    public static void MapAttendeeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/attendees").WithTags("Attendees");

        group.MapGet("/{attendeeId:guid}/availability", CheckAvailability)
            .WithName("CheckAttendeeAvailability")
            .WithSummary("Checks whether an attendee has no Scheduled event overlapping the given time range.")
            .Produces<bool>(StatusCodes.Status200OK);
    }

    private static async Task<IResult> CheckAvailability(
        Guid attendeeId, DateTime startTimeUtc, DateTime endTimeUtc, ISender sender, CancellationToken cancellationToken)
    {
        var isAvailable = await sender.Send(
            new CheckAttendeeAvailabilityQuery(attendeeId, startTimeUtc, endTimeUtc), cancellationToken);

        return Results.Ok(isAvailable);
    }
}
