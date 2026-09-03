using MediatR;
using MyScheduler.Application.Contracts;

namespace MyScheduler.Application.Events.GetEventById;

public sealed record GetEventByIdQuery(Guid EventId) : IRequest<EventDto?>;
