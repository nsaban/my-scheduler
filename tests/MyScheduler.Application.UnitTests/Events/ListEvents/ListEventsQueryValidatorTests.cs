using FluentValidation.TestHelper;
using MyScheduler.Application.Events.ListEvents;

namespace MyScheduler.Application.UnitTests.Events.ListEvents;

public class ListEventsQueryValidatorTests
{
    private readonly ListEventsQueryValidator _validator = new();
    private static readonly DateTime NowUtc = new(2026, 1, 1, 9, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Validate_WhenValid_HasNoErrors()
    {
        var query = new ListEventsQuery(NowUtc, NowUtc.AddDays(1), null, null, null, 1, 20);

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenPageLessThanOne_HasError()
    {
        var query = new ListEventsQuery(null, null, null, null, null, 0, 20);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(q => q.Page);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public void Validate_WhenPageSizeOutOfRange_HasError(int pageSize)
    {
        var query = new ListEventsQuery(null, null, null, null, null, 1, pageSize);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(q => q.PageSize);
    }

    [Fact]
    public void Validate_WhenToBeforeFrom_HasError()
    {
        var query = new ListEventsQuery(NowUtc, NowUtc.AddDays(-1), null, null, null, 1, 20);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(q => q.ToUtc);
    }
}
