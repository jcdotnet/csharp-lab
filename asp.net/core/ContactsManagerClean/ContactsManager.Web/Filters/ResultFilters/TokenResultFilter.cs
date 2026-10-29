using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.Filters.ResultFilters;

/// <summary>
/// This result filter adds an Auth-Key cookie to the HTTP response before the result executes
/// (final changes to the response but they are less common in real-world projects) 
/// </summary>
public class TokenResultFilter : IResultFilter
{
    public void OnResultExecuted(ResultExecutedContext context)
    {
    }

    public void OnResultExecuting(ResultExecutingContext context)
    {
        context.HttpContext.Response.Cookies.Append("Auth-Key", "ExpectedValue");
    }
}
