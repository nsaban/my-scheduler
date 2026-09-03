using MyScheduler.Domain.Common;
using MyScheduler.Domain.ValueObjects;

namespace MyScheduler.Domain.Attendees;

public sealed class Attendee : AggregateRoot
{
    public const int MaxNameLength = 200;

    public string Name { get; private set; } = null!;

    public EmailAddress Email { get; private set; } = null!;

    public DateTime CreatedAtUtc { get; private set; }

    private Attendee()
    {
    }

    public static Attendee Create(string name, EmailAddress email, DateTime nowUtc)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Attendee name is required.");
        }

        if (name.Length > MaxNameLength)
        {
            throw new DomainException($"Attendee name must not exceed {MaxNameLength} characters.");
        }

        return new Attendee
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Email = email,
            CreatedAtUtc = nowUtc,
        };
    }
}
