using FluentValidation.TestHelper;
using MyScheduler.Application.Events.SearchEvents;

namespace MyScheduler.Application.UnitTests.Events.SearchEvents;

public class SearchEventsQueryValidatorTests
{
    private readonly SearchEventsQueryValidator _validator = new();

    [Fact]
    public void Validate_WhenValid_HasNoErrors()
    {
        var result = _validator.TestValidate(new SearchEventsQuery("checkup", 1, 20));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenSearchTermEmpty_HasError()
    {
        var result = _validator.TestValidate(new SearchEventsQuery("", 1, 20));

        result.ShouldHaveValidationErrorFor(q => q.SearchTerm);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public void Validate_WhenPageSizeOutOfRange_HasError(int pageSize)
    {
        var result = _validator.TestValidate(new SearchEventsQuery("checkup", 1, pageSize));

        result.ShouldHaveValidationErrorFor(q => q.PageSize);
    }
}
