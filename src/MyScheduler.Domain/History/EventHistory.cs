using MyScheduler.Domain.Common;

namespace MyScheduler.Domain.History;

public sealed class EventHistory : AggregateRoot
{
    public Guid EventId { get; private set; }

    public int Version { get; private set; }

    public EventChangeType ChangeType { get; private set; }

    public string Snapshot { get; private set; } = null!;

    public Guid? ChangedByAttendeeId { get; private set; }

    public DateTime ChangedAtUtc { get; private set; }

    private EventHistory()
    {
    }

    public static EventHistory Record(
        Guid eventId,
        int version,
        EventChangeType changeType,
        string snapshot,
        Guid? changedByAttendeeId,
        DateTime nowUtc)
    {
        if (string.IsNullOrWhiteSpace(snapshot))
        {
            throw new DomainException("Snapshot is required.");
        }

        return new EventHistory
        {
            Id = Guid.NewGuid(),
            EventId = eventId,
            Version = version,
            ChangeType = changeType,
            Snapshot = snapshot,
            ChangedByAttendeeId = changedByAttendeeId,
            ChangedAtUtc = nowUtc,
        };
    }
}
