using FluentValidation;

namespace MyScheduler.Application.Events.GetEventById;

public sealed class GetEventByIdQueryValidator : AbstractValidator<GetEventByIdQuery>
{
    public GetEventByIdQueryValidator()
    {
        RuleFor(q => q.EventId).NotEmpty();
    }
}
