using FluentValidation;

namespace MyScheduler.Application.Events.CancelEvent;

public sealed class CancelEventCommandValidator : AbstractValidator<CancelEventCommand>
{
    public CancelEventCommandValidator()
    {
        RuleFor(c => c.EventId).NotEmpty();
        RuleFor(c => c.ExpectedVersion).NotEmpty();
    }
}
