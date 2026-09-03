using FluentValidation;
using Microsoft.AspNetCore.Http;
using MyScheduler.Api.Infrastructure;

namespace MyScheduler.Api.UnitTests.Infrastructure;

public class ConcurrencyHelpersTests
{
    [Fact]
    public void ToETag_WrapsBase64InQuotes()
    {
        var rowVersion = new byte[] { 1, 2, 3, 4 };

        var etag = ConcurrencyHelpers.ToETag(rowVersion);

        Assert.Equal($"\"{Convert.ToBase64String(rowVersion)}\"", etag);
    }

    [Fact]
    public void ReadIfMatch_WhenHeaderQuoted_DecodesBase64()
    {
        var rowVersion = new byte[] { 5, 6, 7, 8 };
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["If-Match"] = ConcurrencyHelpers.ToETag(rowVersion);

        var result = ConcurrencyHelpers.ReadIfMatch(httpContext);

        Assert.Equal(rowVersion, result);
    }

    [Fact]
    public void ReadIfMatch_WhenHeaderUnquoted_StillDecodes()
    {
        var rowVersion = new byte[] { 9, 10 };
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["If-Match"] = Convert.ToBase64String(rowVersion);

        var result = ConcurrencyHelpers.ReadIfMatch(httpContext);

        Assert.Equal(rowVersion, result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ReadIfMatch_WhenHeaderMissingOrBlank_ThrowsValidationException(string? headerValue)
    {
        var httpContext = new DefaultHttpContext();

        if (headerValue is not null)
        {
            httpContext.Request.Headers["If-Match"] = headerValue;
        }

        var exception = Assert.Throws<ValidationException>(() => ConcurrencyHelpers.ReadIfMatch(httpContext));
        Assert.Contains(exception.Errors, e => e.PropertyName == "If-Match");
    }
}
