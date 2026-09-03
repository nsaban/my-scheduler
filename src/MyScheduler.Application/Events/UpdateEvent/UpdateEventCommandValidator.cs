using FluentValidation;
using MyScheduler.Domain.Scheduling;

namespace MyScheduler.Application.Events.UpdateEvent;

public sealed class UpdateEventCommandValidator : AbstractValidator<UpdateEventCommand>
{
    public UpdateEventCommandValidator()
    {
        RuleFor(c => c.EventId).NotEmpty();

        RuleFor(c => c.Title)
            .NotEmpty()
            .MaximumLength(Event.MaxTitleLength);

        RuleFor(c => c.Description)
            .MaximumLength(Event.MaxDescriptionLength);

        RuleFor(c => c.EndTimeUtc)
            .GreaterThan(c => c.StartTimeUtc)
            .WithMessage("EndTimeUtc must be after StartTimeUtc.");

        RuleFor(c => c.ExpectedVersion).NotEmpty();
    }
}
