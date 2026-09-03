using FluentValidation.TestHelper;
using MyScheduler.Application.Events.CheckAttendeeAvailability;

namespace MyScheduler.Application.UnitTests.Events.CheckAttendeeAvailability;

public class CheckAttendeeAvailabilityQueryValidatorTests
{
    private readonly CheckAttendeeAvailabilityQueryValidator _validator = new();
    private static readonly DateTime NowUtc = new(2026, 1, 1, 9, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Validate_WhenValid_HasNoErrors()
    {
        var query = new CheckAttendeeAvailabilityQuery(Guid.NewGuid(), NowUtc, NowUtc.AddHours(1));

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenAttendeeIdEmpty_HasError()
    {
        var query = new CheckAttendeeAvailabilityQuery(Guid.Empty, NowUtc, NowUtc.AddHours(1));

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(q => q.AttendeeId);
    }

    [Fact]
    public void Validate_WhenEndNotAfterStart_HasError()
    {
        var query = new CheckAttendeeAvailabilityQuery(Guid.NewGuid(), NowUtc, NowUtc);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(q => q.EndTimeUtc);
    }
}
