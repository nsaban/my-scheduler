using System.Text.Json;
using Microsoft.AspNetCore.Http;
using MyScheduler.Api.Infrastructure;
using MyScheduler.Application.Common;

namespace MyScheduler.Api.UnitTests.Infrastructure;

public class NotFoundExceptionHandlerTests
{
    private readonly NotFoundExceptionHandler _handler = new();

    [Fact]
    public async Task TryHandleAsync_WhenNotNotFoundException_ReturnsFalse()
    {
        var httpContext = new DefaultHttpContext();

        var handled = await _handler.TryHandleAsync(httpContext, new InvalidOperationException(), CancellationToken.None);

        Assert.False(handled);
        Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
    }

    [Fact]
    public async Task TryHandleAsync_WhenNotFoundException_Writes404WithMessageAsTitle()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();

        var handled = await _handler.TryHandleAsync(
            httpContext, new NotFoundException("Attendee 'x' was not found."), CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status404NotFound, httpContext.Response.StatusCode);

        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        using var document = await JsonDocument.ParseAsync(httpContext.Response.Body);
        Assert.Equal("Attendee 'x' was not found.", document.RootElement.GetProperty("title").GetString());
        Assert.Equal(404, document.RootElement.GetProperty("status").GetInt32());
    }
}
