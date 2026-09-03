using MediatR;
using MyScheduler.Application.Contracts;
using MyScheduler.Domain.Scheduling;

namespace MyScheduler.Application.Events.ListEvents;

public sealed record ListEventsQuery(
    DateTime? FromUtc,
    DateTime? ToUtc,
    EventStatus? Status,
    Guid? OrganizerId,
    Guid? AttendeeId,
    int Page,
    int PageSize) : IRequest<PagedResult<EventSummaryDto>>;
