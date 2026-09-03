using FluentValidation;
using MyScheduler.Domain.Scheduling;

namespace MyScheduler.Application.Events.CreateEvent;

public sealed class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
{
    public CreateEventCommandValidator()
    {
        RuleFor(c => c.Title)
            .NotEmpty()
            .MaximumLength(Event.MaxTitleLength);

        RuleFor(c => c.Description)
            .MaximumLength(Event.MaxDescriptionLength);

        RuleFor(c => c.EndTimeUtc)
            .GreaterThan(c => c.StartTimeUtc)
            .WithMessage("EndTimeUtc must be after StartTimeUtc.");

        RuleFor(c => c.OrganizerId)
            .NotEmpty();
    }
}
