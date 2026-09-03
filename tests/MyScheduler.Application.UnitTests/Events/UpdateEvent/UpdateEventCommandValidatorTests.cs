using FluentValidation.TestHelper;
using MyScheduler.Application.Events.UpdateEvent;
using MyScheduler.Domain.Scheduling;

namespace MyScheduler.Application.UnitTests.Events.UpdateEvent;

public class UpdateEventCommandValidatorTests
{
    private readonly UpdateEventCommandValidator _validator = new();
    private static readonly DateTime NowUtc = new(2026, 1, 1, 9, 0, 0, DateTimeKind.Utc);

    private static UpdateEventCommand ValidCommand() => new(
        Guid.NewGuid(), "Checkup", "Notes", NowUtc, NowUtc.AddHours(1), [1, 2, 3]);

    [Fact]
    public void Validate_WhenValid_HasNoErrors()
    {
        var result = _validator.TestValidate(ValidCommand());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenEventIdEmpty_HasError()
    {
        var command = ValidCommand() with { EventId = Guid.Empty };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.EventId);
    }

    [Fact]
    public void Validate_WhenTitleTooLong_HasError()
    {
        var command = ValidCommand() with { Title = new string('a', Event.MaxTitleLength + 1) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Title);
    }

    [Fact]
    public void Validate_WhenEndNotAfterStart_HasError()
    {
        var command = ValidCommand() with { EndTimeUtc = NowUtc };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.EndTimeUtc);
    }

    [Fact]
    public void Validate_WhenExpectedVersionEmpty_HasError()
    {
        var command = ValidCommand() with { ExpectedVersion = [] };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.ExpectedVersion);
    }
}
