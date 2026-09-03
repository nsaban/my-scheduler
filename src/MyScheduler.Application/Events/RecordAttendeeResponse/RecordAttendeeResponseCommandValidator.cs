using FluentValidation;

namespace MyScheduler.Application.Events.RecordAttendeeResponse;

public sealed class RecordAttendeeResponseCommandValidator : AbstractValidator<RecordAttendeeResponseCommand>
{
    public RecordAttendeeResponseCommandValidator()
    {
        RuleFor(c => c.EventId).NotEmpty();
        RuleFor(c => c.AttendeeId).NotEmpty();
        RuleFor(c => c.Response).IsInEnum();
        RuleFor(c => c.ExpectedVersion).NotEmpty();
    }
}
