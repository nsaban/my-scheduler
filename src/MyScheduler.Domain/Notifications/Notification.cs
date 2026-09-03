using MyScheduler.Domain.Common;

namespace MyScheduler.Domain.Notifications;

public sealed class Notification : AggregateRoot
{
    public const int MaxLastErrorMessageLength = 1000;

    // After this many failed send attempts, a notification stops being retried.
    private const int MaxAttempts = 5;

    public Guid EventId { get; private set; }

    public Guid RecipientAttendeeId { get; private set; }

    public NotificationChannel Channel { get; private set; }

    public NotificationTriggerType TriggerType { get; private set; }

    public string Payload { get; private set; } = null!;

    public NotificationStatus Status { get; private set; }

    public int Attempts { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? SentAtUtc { get; private set; }

    public string? LastErrorMessage { get; private set; }

    private Notification()
    {
    }

    public static Notification Create(
        Guid eventId,
        Guid recipientAttendeeId,
        NotificationChannel channel,
        NotificationTriggerType triggerType,
        string payload,
        DateTime nowUtc)
    {
        if (string.IsNullOrWhiteSpace(payload))
        {
            throw new DomainException("Notification payload is required.");
        }

        return new Notification
        {
            Id = Guid.NewGuid(),
            EventId = eventId,
            RecipientAttendeeId = recipientAttendeeId,
            Channel = channel,
            TriggerType = triggerType,
            Payload = payload,
            Status = NotificationStatus.Pending,
            Attempts = 0,
            CreatedAtUtc = nowUtc,
        };
    }

    public void MarkSent(DateTime nowUtc)
    {
        if (Status == NotificationStatus.Sent)
        {
            throw new DomainException("Notification is already sent.");
        }

        Attempts++;
        Status = NotificationStatus.Sent;
        SentAtUtc = nowUtc;
        LastErrorMessage = null;
    }

    public void MarkFailed(string errorMessage)
    {
        if (Status == NotificationStatus.Sent)
        {
            throw new DomainException("Cannot fail a notification that has already been sent.");
        }

        Attempts++;
        LastErrorMessage = Truncate(errorMessage, MaxLastErrorMessageLength);
        Status = Attempts >= MaxAttempts ? NotificationStatus.Failed : NotificationStatus.Pending;
    }

    private static string Truncate(string value, int maxLength) =>
        value.Length <= maxLength ? value : value[..maxLength];
}
