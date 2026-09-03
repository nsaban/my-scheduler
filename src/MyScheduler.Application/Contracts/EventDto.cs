using MyScheduler.Domain.Scheduling;

namespace MyScheduler.Application.Contracts;

public sealed record EventDto
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string? Description { get; init; }

    public DateTime StartTimeUtc { get; init; }

    public DateTime EndTimeUtc { get; init; }

    public EventStatus Status { get; init; }

    public Guid OrganizerId { get; init; }

    public DateTime CreatedAtUtc { get; init; }

    public DateTime UpdatedAtUtc { get; init; }

    /// <summary>Opaque optimistic-concurrency token; round-tripped by clients as an ETag/If-Match value.</summary>
    public byte[] RowVersion { get; init; } = [];

    public IReadOnlyCollection<EventAttendeeDto> Attendees { get; init; } = [];
}
