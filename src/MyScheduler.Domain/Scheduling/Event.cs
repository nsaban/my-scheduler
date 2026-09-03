using MyScheduler.Domain.Common;
using MyScheduler.Domain.Scheduling.DomainEvents;
using MyScheduler.Domain.ValueObjects;

namespace MyScheduler.Domain.Scheduling;

public sealed class Event : AggregateRoot
{
    public const int MaxTitleLength = 200;
    public const int MaxDescriptionLength = 2000;

    private readonly List<EventAttendee> _eventAttendees = [];

    public string Title { get; private set; } = null!;

    public string? Description { get; private set; }

    public DateTimeRange TimeRange { get; private set; } = null!;

    public EventStatus Status { get; private set; }

    public Guid OrganizerId { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    public IReadOnlyCollection<EventAttendee> EventAttendees => _eventAttendees.AsReadOnly();

    private Event()
    {
    }

    public static Event Schedule(
        string title,
        string? description,
        DateTimeRange timeRange,
        Guid organizerId,
        IEnumerable<Guid> attendeeIds,
        DateTime nowUtc)
    {
        ValidateTitle(title);
        ValidateDescription(description);

        var @event = new Event
        {
            Id = Guid.NewGuid(),
            Title = title.Trim(),
            Description = NormalizeDescription(description),
            TimeRange = timeRange,
            Status = EventStatus.Scheduled,
            OrganizerId = organizerId,
            CreatedAtUtc = nowUtc,
            UpdatedAtUtc = nowUtc,
        };

        var invitedAttendeeIds = new HashSet<Guid>(attendeeIds) { organizerId };
        foreach (var attendeeId in invitedAttendeeIds)
        {
            @event._eventAttendees.Add(new EventAttendee(@event.Id, attendeeId));
        }

        @event.AddDomainEvent(new EventCreatedDomainEvent(@event.Id, nowUtc));

        return @event;
    }

    public void UpdateDetails(string title, string? description, DateTimeRange timeRange, DateTime nowUtc)
    {
        EnsureNotCancelled();
        ValidateTitle(title);
        ValidateDescription(description);

        Title = title.Trim();
        Description = NormalizeDescription(description);
        TimeRange = timeRange;
        UpdatedAtUtc = nowUtc;

        AddDomainEvent(new EventUpdatedDomainEvent(Id, nowUtc));
    }

    public void Cancel(DateTime nowUtc)
    {
        if (Status == EventStatus.Cancelled)
        {
            throw new DomainException("Event is already cancelled.");
        }

        Status = EventStatus.Cancelled;
        UpdatedAtUtc = nowUtc;

        AddDomainEvent(new EventCancelledDomainEvent(Id, nowUtc));
    }

    public void RecordAttendeeResponse(Guid attendeeId, ResponseStatus response, DateTime nowUtc)
    {
        EnsureNotCancelled();

        var eventAttendee = _eventAttendees.SingleOrDefault(ea => ea.AttendeeId == attendeeId)
            ?? throw new DomainException("Attendee is not invited to this event.");

        eventAttendee.RecordResponse(response, nowUtc);

        AddDomainEvent(new AttendeeResponseRecordedDomainEvent(Id, attendeeId, response, nowUtc));
    }

    private void EnsureNotCancelled()
    {
        if (Status == EventStatus.Cancelled)
        {
            throw new DomainException("Event is cancelled.");
        }
    }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("Event title is required.");
        }

        if (title.Length > MaxTitleLength)
        {
            throw new DomainException($"Event title must not exceed {MaxTitleLength} characters.");
        }
    }

    private static void ValidateDescription(string? description)
    {
        if (description is { Length: > MaxDescriptionLength })
        {
            throw new DomainException($"Event description must not exceed {MaxDescriptionLength} characters.");
        }
    }

    private static string? NormalizeDescription(string? description) =>
        string.IsNullOrWhiteSpace(description) ? null : description.Trim();
}
