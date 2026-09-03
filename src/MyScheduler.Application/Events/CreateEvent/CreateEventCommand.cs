using MediatR;

namespace MyScheduler.Application.Events.CreateEvent;

public sealed record CreateEventCommand(
    string Title,
    string? Description,
    DateTime StartTimeUtc,
    DateTime EndTimeUtc,
    Guid OrganizerId,
    IReadOnlyCollection<Guid> AttendeeIds) : IRequest<Guid>;
