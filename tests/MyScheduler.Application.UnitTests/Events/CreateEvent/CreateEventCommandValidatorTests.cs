using FluentValidation.TestHelper;
using MyScheduler.Application.Events.CreateEvent;
using MyScheduler.Domain.Scheduling;

namespace MyScheduler.Application.UnitTests.Events.CreateEvent;

public class CreateEventCommandValidatorTests
{
    private readonly CreateEventCommandValidator _validator = new();
    private static readonly DateTime NowUtc = new(2026, 1, 1, 9, 0, 0, DateTimeKind.Utc);

    private static CreateEventCommand ValidCommand() => new(
        "Checkup", "Notes", NowUtc, NowUtc.AddHours(1), Guid.NewGuid(), []);

    [Fact]
    public void Validate_WhenValid_HasNoErrors()
    {
        var result = _validator.TestValidate(ValidCommand());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenTitleEmpty_HasError()
    {
        var command = ValidCommand() with { Title = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Title);
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
    public void Validate_WhenOrganizerIdEmpty_HasError()
    {
        var command = ValidCommand() with { OrganizerId = Guid.Empty };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.OrganizerId);
    }
}
