using FluentValidation;

namespace MyScheduler.Application.Events.CheckAttendeeAvailability;

public sealed class CheckAttendeeAvailabilityQueryValidator : AbstractValidator<CheckAttendeeAvailabilityQuery>
{
    public CheckAttendeeAvailabilityQueryValidator()
    {
        RuleFor(q => q.AttendeeId).NotEmpty();
        RuleFor(q => q.EndTimeUtc).GreaterThan(q => q.StartTimeUtc);
    }
}
