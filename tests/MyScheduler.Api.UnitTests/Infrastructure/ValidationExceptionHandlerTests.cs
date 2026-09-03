using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using MyScheduler.Api.Infrastructure;

namespace MyScheduler.Api.UnitTests.Infrastructure;

public class ValidationExceptionHandlerTests
{
    private readonly ValidationExceptionHandler _handler = new();

    [Fact]
    public async Task TryHandleAsync_WhenNotValidationException_ReturnsFalse()
    {
        var httpContext = new DefaultHttpContext();

        var handled = await _handler.TryHandleAsync(httpContext, new InvalidOperationException(), CancellationToken.None);

        Assert.False(handled);
        Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
    }

    [Fact]
    public async Task TryHandleAsync_WhenValidationException_Writes400WithGroupedErrors()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();

        var exception = new ValidationException(
        [
            new ValidationFailure("Title", "Title is required."),
            new ValidationFailure("Title", "Title must not exceed 200 characters."),
            new ValidationFailure("EndTimeUtc", "EndTimeUtc must be after StartTimeUtc."),
        ]);

        var handled = await _handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status400BadRequest, httpContext.Response.StatusCode);

        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        using var document = await JsonDocument.ParseAsync(httpContext.Response.Body);
        var errors = document.RootElement.GetProperty("errors");

        Assert.Equal(2, errors.GetProperty("Title").GetArrayLength());
        Assert.Equal(1, errors.GetProperty("EndTimeUtc").GetArrayLength());
    }
}
