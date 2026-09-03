using MyScheduler.Domain.Scheduling;

namespace MyScheduler.Application.Contracts;

public sealed record EventAttendeeDto
{
    public Guid AttendeeId { get; init; }

    public ResponseStatus ResponseStatus { get; init; }

    public DateTime? RespondedAtUtc { get; init; }
}
