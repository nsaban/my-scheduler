using FluentValidation;

namespace MyScheduler.Application.Events.SearchEvents;

public sealed class SearchEventsQueryValidator : AbstractValidator<SearchEventsQuery>
{
    public SearchEventsQueryValidator()
    {
        RuleFor(q => q.SearchTerm).NotEmpty();
        RuleFor(q => q.Page).GreaterThanOrEqualTo(1);
        RuleFor(q => q.PageSize).InclusiveBetween(1, 100);
    }
}
