using MediatR;

namespace MyScheduler.Application.Events.CancelEvent;

public sealed record CancelEventCommand(Guid EventId, byte[] ExpectedVersion) : IRequest;
