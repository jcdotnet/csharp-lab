using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Noticore.Api.Middleware
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,
            Exception exception, CancellationToken cancellationToken
        )
        {
            logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

            // Standardized Error Response (RFC 7807)
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Server Error",
                Type = "https://ietf.org",
                Detail = exception.Message // In production, don't send the full message for security
            };

            httpContext.Response.StatusCode = problemDetails.Status.Value;

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true; // The exception is handled
        }
    }
}
