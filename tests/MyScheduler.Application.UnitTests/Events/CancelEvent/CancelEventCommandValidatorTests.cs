using FluentValidation.TestHelper;
using MyScheduler.Application.Events.CancelEvent;

namespace MyScheduler.Application.UnitTests.Events.CancelEvent;

public class CancelEventCommandValidatorTests
{
    private readonly CancelEventCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenValid_HasNoErrors()
    {
        var result = _validator.TestValidate(new CancelEventCommand(Guid.NewGuid(), [1, 2, 3]));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenEventIdEmpty_HasError()
    {
        var result = _validator.TestValidate(new CancelEventCommand(Guid.Empty, [1, 2, 3]));

        result.ShouldHaveValidationErrorFor(c => c.EventId);
    }

    [Fact]
    public void Validate_WhenExpectedVersionEmpty_HasError()
    {
        var result = _validator.TestValidate(new CancelEventCommand(Guid.NewGuid(), []));

        result.ShouldHaveValidationErrorFor(c => c.ExpectedVersion);
    }
}
