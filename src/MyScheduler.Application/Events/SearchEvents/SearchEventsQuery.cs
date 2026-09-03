using MediatR;
using MyScheduler.Application.Contracts;

namespace MyScheduler.Application.Events.SearchEvents;

public sealed record SearchEventsQuery(string SearchTerm, int Page, int PageSize) : IRequest<PagedResult<EventSummaryDto>>;
