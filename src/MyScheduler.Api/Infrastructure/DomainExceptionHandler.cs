using Microsoft.AspNetCore.Diagnostics;
using MyScheduler.Domain.Common;

namespace MyScheduler.Api.Infrastructure;

public sealed class DomainExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not DomainException domainException)
        {
            return false;
        }

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        await httpContext.Response.WriteAsJsonAsync(
            new
            {
                title = domainException.Message,
                status = StatusCodes.Status400BadRequest,
            },
            cancellationToken);

        return true;
    }
}
