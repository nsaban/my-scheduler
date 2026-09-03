using System.Text.Json;
using Microsoft.AspNetCore.Http;
using MyScheduler.Api.Infrastructure;
using MyScheduler.Domain.Common;

namespace MyScheduler.Api.UnitTests.Infrastructure;

public class DomainExceptionHandlerTests
{
    private readonly DomainExceptionHandler _handler = new();

    [Fact]
    public async Task TryHandleAsync_WhenNotDomainException_ReturnsFalse()
    {
        var httpContext = new DefaultHttpContext();

        var handled = await _handler.TryHandleAsync(httpContext, new InvalidOperationException(), CancellationToken.None);

        Assert.False(handled);
        Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
    }

    [Fact]
    public async Task TryHandleAsync_WhenDomainException_Writes400WithMessageAsTitle()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();

        var handled = await _handler.TryHandleAsync(
            httpContext, new DomainException("Event is already cancelled."), CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status400BadRequest, httpContext.Response.StatusCode);

        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        using var document = await JsonDocument.ParseAsync(httpContext.Response.Body);
        Assert.Equal("Event is already cancelled.", document.RootElement.GetProperty("title").GetString());
    }
}
