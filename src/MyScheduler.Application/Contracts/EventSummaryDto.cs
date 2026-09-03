using MyScheduler.Domain.Scheduling;

namespace MyScheduler.Application.Contracts;

public sealed record EventSummaryDto
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public DateTime StartTimeUtc { get; init; }

    public DateTime EndTimeUtc { get; init; }

    public EventStatus Status { get; init; }

    public Guid OrganizerId { get; init; }
}
