using MediatR;

namespace MyScheduler.Application.Events.UpdateEvent;

public sealed record UpdateEventCommand(
    Guid EventId,
    string Title,
    string? Description,
    DateTime StartTimeUtc,
    DateTime EndTimeUtc,
    byte[] ExpectedVersion) : IRequest;
