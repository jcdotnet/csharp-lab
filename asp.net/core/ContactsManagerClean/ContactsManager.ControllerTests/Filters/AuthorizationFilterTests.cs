using ContactsManager.Filters.AuthorizationFilters;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace ContactsManager.ControllerTests.Filters;

public class AuthorizationFilterTests
{
    private readonly ActionContext _actionContext;
    private readonly TokenAuthorizationFilter _filter = new();

    public AuthorizationFilterTests()
    {
        _actionContext = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
    }

    [Fact]
    public void OnAuthorization_SetsUnauthorizedResult_WhenAuthCookieIsMissing()
    {
        // Arrange
        AuthorizationFilterContext context = new(_actionContext, []);

        // Act
        _filter.OnAuthorization(context);

        // Assert
        context.Result.Should().BeOfType<StatusCodeResult>().Which
            .StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public void OnAuthorization_DoesNotSetResult_WhenAuthCookieIsValid()
    {
        // Arrange
        _actionContext.HttpContext.Request.Headers.Cookie = "Auth-Key=ExpectedValue";
        
        AuthorizationFilterContext context = new(_actionContext, []);

        // Act
        _filter.OnAuthorization(context);

        // Assert
        context.Result.Should().BeNull();
    }
}
