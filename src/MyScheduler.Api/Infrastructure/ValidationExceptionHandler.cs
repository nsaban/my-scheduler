using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace MyScheduler.Api.Infrastructure;

public sealed class ValidationExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validationException)
        {
            return false;
        }

        var errors = validationException.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        await httpContext.Response.WriteAsJsonAsync(
            new
            {
                title = "One or more validation errors occurred.",
                status = StatusCodes.Status400BadRequest,
                errors,
            },
            cancellationToken);

        return true;
    }
}
