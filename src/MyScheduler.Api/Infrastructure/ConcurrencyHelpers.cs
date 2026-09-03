using FluentValidation;
using FluentValidation.Results;

namespace MyScheduler.Api.Infrastructure;

public static class ConcurrencyHelpers
{
    public static string ToETag(byte[] rowVersion) => $"\"{Convert.ToBase64String(rowVersion)}\"";

    public static byte[] ReadIfMatch(HttpContext httpContext)
    {
        var headerValue = httpContext.Request.Headers["If-Match"].ToString();

        if (string.IsNullOrWhiteSpace(headerValue))
        {
            throw new ValidationException(
                [new ValidationFailure("If-Match", "An If-Match header with the resource's current ETag is required.")]);
        }

        return Convert.FromBase64String(headerValue.Trim('"'));
    }
}
