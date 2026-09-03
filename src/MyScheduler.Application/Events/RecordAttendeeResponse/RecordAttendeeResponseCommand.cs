using MediatR;
using MyScheduler.Domain.Scheduling;

namespace MyScheduler.Application.Events.RecordAttendeeResponse;

public sealed record RecordAttendeeResponseCommand(
    Guid EventId,
    Guid AttendeeId,
    ResponseStatus Response,
    byte[] ExpectedVersion) : IRequest;
