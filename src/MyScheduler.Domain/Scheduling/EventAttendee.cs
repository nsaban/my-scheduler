namespace MyScheduler.Domain.Scheduling;

public sealed class EventAttendee
{
    public Guid EventId { get; private set; }

    public Guid AttendeeId { get; private set; }

    public ResponseStatus ResponseStatus { get; private set; }

    public DateTime? RespondedAtUtc { get; private set; }

    private EventAttendee()
    {
    }

    internal EventAttendee(Guid eventId, Guid attendeeId)
    {
        EventId = eventId;
        AttendeeId = attendeeId;
        ResponseStatus = ResponseStatus.Pending;
    }

    internal void RecordResponse(ResponseStatus response, DateTime respondedAtUtc)
    {
        ResponseStatus = response;
        RespondedAtUtc = respondedAtUtc;
    }
}
