using FluentValidation;

namespace MyScheduler.Application.Events.ListEvents;

public sealed class ListEventsQueryValidator : AbstractValidator<ListEventsQuery>
{
    public ListEventsQueryValidator()
    {
        RuleFor(q => q.Page).GreaterThanOrEqualTo(1);
        RuleFor(q => q.PageSize).InclusiveBetween(1, 100);

        RuleFor(q => q.ToUtc)
            .GreaterThan(q => q.FromUtc)
            .When(q => q.FromUtc.HasValue && q.ToUtc.HasValue)
            .WithMessage("ToUtc must be after FromUtc.");
    }
}
