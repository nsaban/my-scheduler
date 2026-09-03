using FluentValidation.TestHelper;
using MyScheduler.Application.Events.RecordAttendeeResponse;
using MyScheduler.Domain.Scheduling;

namespace MyScheduler.Application.UnitTests.Events.RecordAttendeeResponse;

public class RecordAttendeeResponseCommandValidatorTests
{
    private readonly RecordAttendeeResponseCommandValidator _validator = new();

    private static RecordAttendeeResponseCommand ValidCommand() =>
        new(Guid.NewGuid(), Guid.NewGuid(), ResponseStatus.Accepted, [1, 2, 3]);

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
    public void Validate_WhenAttendeeIdEmpty_HasError()
    {
        var command = ValidCommand() with { AttendeeId = Guid.Empty };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.AttendeeId);
    }

    [Fact]
    public void Validate_WhenResponseNotInEnum_HasError()
    {
        var command = ValidCommand() with { Response = (ResponseStatus)999 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Response);
    }
}
