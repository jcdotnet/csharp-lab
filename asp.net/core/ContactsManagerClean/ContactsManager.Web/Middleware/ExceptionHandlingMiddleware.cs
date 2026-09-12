using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.Middleware
{
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await next(httpContext); // reference of subsequent middleware
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred");

                // Return a standardized API error response instead of writing plain text

                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

                await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "An unexpected error occurred"
                });
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ExceptionHandlingMiddlewareExtensions
    {
        extension(IApplicationBuilder builder)
        {
            public IApplicationBuilder UseExceptionHandlingMiddleware()
            {
                return builder.UseMiddleware<ExceptionHandlingMiddleware>();
            }
        }
    }
}