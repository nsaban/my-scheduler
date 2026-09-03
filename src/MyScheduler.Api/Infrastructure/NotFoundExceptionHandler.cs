using Microsoft.AspNetCore.Diagnostics;
using MyScheduler.Application.Common;

namespace MyScheduler.Api.Infrastructure;

public sealed class NotFoundExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not NotFoundException notFoundException)
        {
            return false;
        }

        httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
        await httpContext.Response.WriteAsJsonAsync(
            new
            {
                title = notFoundException.Message,
                status = StatusCodes.Status404NotFound,
            },
            cancellationToken);

        return true;
    }
}
