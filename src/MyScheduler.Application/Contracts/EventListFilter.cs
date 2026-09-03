using MyScheduler.Domain.Scheduling;

namespace MyScheduler.Application.Contracts;

public sealed record EventListFilter
{
    public DateTime? FromUtc { get; init; }

    public DateTime? ToUtc { get; init; }

    public EventStatus? Status { get; init; }

    public Guid? OrganizerId { get; init; }

    public Guid? AttendeeId { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;
}
