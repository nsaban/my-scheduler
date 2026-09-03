using MediatR;
using MyScheduler.Api.Contracts.Requests;
using MyScheduler.Api.Infrastructure;
using MyScheduler.Application.Contracts;
using MyScheduler.Application.Events.CancelEvent;
using MyScheduler.Application.Events.CreateEvent;
using MyScheduler.Application.Events.GetEventById;
using MyScheduler.Application.Events.ListEvents;
using MyScheduler.Application.Events.RecordAttendeeResponse;
using MyScheduler.Application.Events.SearchEvents;
using MyScheduler.Application.Events.UpdateEvent;
using MyScheduler.Domain.Scheduling;

namespace MyScheduler.Api.Endpoints;

public static class EventEndpoints
{
    public static void MapEventEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/events").WithTags("Events");

        group.MapPost("/", CreateEvent)
            .WithName("CreateEvent")
            .WithSummary("Schedules a new event.")
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        group.MapGet("/{id:guid}", GetEventById)
            .WithName("GetEventById")
            .WithSummary("Gets a single event by id. The response ETag is required as the If-Match header on subsequent writes.")
            .Produces<EventDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", ListEvents)
            .WithName("ListEvents")
            .WithSummary("Lists events with optional date range, status, organizer and attendee filters.")
            .Produces<PagedResult<EventSummaryDto>>(StatusCodes.Status200OK);

        group.MapGet("/search", SearchEvents)
            .WithName("SearchEvents")
            .WithSummary("Full-text searches events by title/description.")
            .Produces<PagedResult<EventSummaryDto>>(StatusCodes.Status200OK);

        group.MapPut("/{id:guid}", UpdateEvent)
            .WithName("UpdateEvent")
            .WithSummary("Updates an event's details. Requires an If-Match header with the event's current ETag.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapDelete("/{id:guid}", CancelEvent)
            .WithName("CancelEvent")
            .WithSummary("Cancels an event (soft delete, never a physical row delete). Requires an If-Match header.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/{eventId:guid}/attendees/{attendeeId:guid}/response", RecordAttendeeResponse)
            .WithName("RecordAttendeeResponse")
            .WithSummary("Records an attendee's Accept/Decline/Tentative response. Requires an If-Match header.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> CreateEvent(CreateEventCommand command, ISender sender, CancellationToken cancellationToken)
    {
        var eventId = await sender.Send(command, cancellationToken);
        return Results.Created($"/events/{eventId}", eventId);
    }

    private static async Task<IResult> GetEventById(
        Guid id, HttpContext httpContext, ISender sender, CancellationToken cancellationToken)
    {
        var dto = await sender.Send(new GetEventByIdQuery(id), cancellationToken);

        if (dto is null)
        {
            return Results.NotFound();
        }

        httpContext.Response.Headers["ETag"] = ConcurrencyHelpers.ToETag(dto.RowVersion);
        return Results.Ok(dto);
    }

    private static async Task<IResult> ListEvents(
        ISender sender,
        CancellationToken cancellationToken,
        DateTime? fromUtc = null,
        DateTime? toUtc = null,
        EventStatus? status = null,
        Guid? organizerId = null,
        Guid? attendeeId = null,
        int page = 1,
        int pageSize = 20)
    {
        var result = await sender.Send(
            new ListEventsQuery(fromUtc, toUtc, status, organizerId, attendeeId, page, pageSize), cancellationToken);

        return Results.Ok(result);
    }

    private static async Task<IResult> SearchEvents(
        string q, ISender sender, CancellationToken cancellationToken, int page = 1, int pageSize = 20)
    {
        var result = await sender.Send(new SearchEventsQuery(q, page, pageSize), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> UpdateEvent(
        Guid id, UpdateEventRequest request, HttpContext httpContext, ISender sender, CancellationToken cancellationToken)
    {
        var expectedVersion = ConcurrencyHelpers.ReadIfMatch(httpContext);
        var command = new UpdateEventCommand(id, request.Title, request.Description, request.StartTimeUtc, request.EndTimeUtc, expectedVersion);

        await sender.Send(command, cancellationToken);

        return Results.NoContent();
    }

    private static async Task<IResult> CancelEvent(
        Guid id, HttpContext httpContext, ISender sender, CancellationToken cancellationToken)
    {
        var expectedVersion = ConcurrencyHelpers.ReadIfMatch(httpContext);
        await sender.Send(new CancelEventCommand(id, expectedVersion), cancellationToken);

        return Results.NoContent();
    }

    private static async Task<IResult> RecordAttendeeResponse(
        Guid eventId,
        Guid attendeeId,
        RecordAttendeeResponseRequest request,
        HttpContext httpContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var expectedVersion = ConcurrencyHelpers.ReadIfMatch(httpContext);
        var command = new RecordAttendeeResponseCommand(eventId, attendeeId, request.Response, expectedVersion);

        await sender.Send(command, cancellationToken);

        return Results.NoContent();
    }
}
