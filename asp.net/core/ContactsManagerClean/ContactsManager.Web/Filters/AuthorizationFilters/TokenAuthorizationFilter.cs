using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.Filters.AuthorizationFilters;

/// <summary>
/// This authorization filter checks the Auth-Key cookie before allowing access
/// and returns an unauthorized response if the cookie is missing or invalid
// (this filter is used here to demonstrate how IAuthorizationFilter works,
// since ASP.NET Core provides built-in authentication and authorization)
/// </summary>
public class TokenAuthorizationFilter : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.HttpContext.Request.Cookies.ContainsKey("Auth-Key") == false ||
            context.HttpContext.Request.Cookies["Auth-Key"] != "ExpectedValue")
        {
            context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
            //return;
        }
    }
}
