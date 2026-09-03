using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyScheduler.Api.Infrastructure;

namespace MyScheduler.Api.UnitTests.Infrastructure;

public class ConcurrencyExceptionHandlerTests
{
    private readonly ConcurrencyExceptionHandler _handler = new();

    [Fact]
    public async Task TryHandleAsync_WhenNotConcurrencyException_ReturnsFalseAndLeavesResponseUntouched()
    {
        var httpContext = new DefaultHttpContext();

        var handled = await _handler.TryHandleAsync(httpContext, new InvalidOperationException(), CancellationToken.None);

        Assert.False(handled);
        Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
    }

    [Fact]
    public async Task TryHandleAsync_WhenConcurrencyException_Writes409WithProblemBody()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();

        var handled = await _handler.TryHandleAsync(httpContext, new DbUpdateConcurrencyException("conflict"), CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status409Conflict, httpContext.Response.StatusCode);

        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        using var document = await JsonDocument.ParseAsync(httpContext.Response.Body);
        Assert.Equal(409, document.RootElement.GetProperty("status").GetInt32());
    }
}
