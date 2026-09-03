using System.Text.RegularExpressions;
using MyScheduler.Domain.Common;

namespace MyScheduler.Domain.ValueObjects;

public sealed partial record EmailAddress
{
    public const int MaxLength = 254;

    public string Value { get; }

    private EmailAddress(string value) => Value = value;

    public static EmailAddress Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("Email is required.");
        }

        value = value.Trim();

        if (value.Length > MaxLength)
        {
            throw new DomainException($"Email must not exceed {MaxLength} characters.");
        }

        if (!EmailRegex().IsMatch(value))
        {
            throw new DomainException("Email is not a valid email address.");
        }

        return new EmailAddress(value);
    }

    public override string ToString() => Value;

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}
