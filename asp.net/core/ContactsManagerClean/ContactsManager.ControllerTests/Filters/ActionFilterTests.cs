using ContactsManager.Core.DTO;
using ContactsManager.Enums;
using ContactsManager.Filters.ActionFilters;
using ContactsManager.Web.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceContracts;

namespace ContactsManager.ControllerTests.Filters;

public class ActionFilterTests
{
    private readonly Mock<ICountriesGetterService> _countriesServiceMock = new();
    private readonly ContactsController _controller;
    private readonly DefaultHttpContext _httpContext = new();
    private readonly ActionContext _actionContext;

    private readonly ContactsHttpPostActionFilter _contactsHttpPostFilter;
    private readonly ContactsListActionFilter _contactsListFilter;
    private readonly ResponseHeaderActionFilter _responseHeaderFilter;

    public ActionFilterTests()
    {
        _controller = new(
            Mock.Of<IContactsAdderService>(),
            Mock.Of<IContactsGetterService>(),
            Mock.Of<IContactsUpdaterService>(),
            Mock.Of<IContactsSorterService>(),
            Mock.Of<IContactsDeleterService>(),
            _countriesServiceMock.Object
        );
        _actionContext = new ActionContext(_httpContext, new RouteData(), new ActionDescriptor());

        _contactsHttpPostFilter = new ContactsHttpPostActionFilter(
            Mock.Of<ILogger<ContactsHttpPostActionFilter>>(),
            _countriesServiceMock.Object
        );
        _contactsListFilter = new ContactsListActionFilter(Mock.Of<ILogger<ContactsListActionFilter>>());
        _responseHeaderFilter = new ResponseHeaderActionFilter(
            Mock.Of<ILogger<ResponseHeaderActionFilter>>(),
            "Test-Header",
            "TestValue"
        );
    }

    #region ContactsHttpPostFilter

    [Fact]
    public async Task OnActionExecuting_DoesNotSetResult_WhenModelStateIsValid()
    {
        // Arrange
        var actionArguments = new Dictionary<string, object?>()
        {
            ["personRequest"] = new PersonAddRequest()
        };

        var actionExecutingContext = new ActionExecutingContext(_actionContext, [], actionArguments, _controller);
        var actionExecutedContext = new ActionExecutedContext(_actionContext, [], _controller);

        // FromResult avoids unnecesary async work here
        ActionExecutionDelegate next = () => Task.FromResult(actionExecutedContext);

        // Act
        await _contactsHttpPostFilter.OnActionExecutionAsync(actionExecutingContext, next);

        // Assert
        actionExecutingContext.Result.Should().BeNull();
    }

    [Fact]
    public async Task OnActionExecuting_SetsResult_WhenModelStateIsInvalid()
    {
        // Arrange
        var actionArguments = new Dictionary<string, object?>()
        {
            ["personRequest"] = new PersonAddRequest()
        };

        _countriesServiceMock.Setup(service => service.GetCountries()).ReturnsAsync(new List<CountryResponse>());

        _controller.ModelState.AddModelError("Name", "Name is required");

        var actionExecutingContext = new ActionExecutingContext(_actionContext, [], actionArguments, _controller);
        var actionExecutedContext = new ActionExecutedContext(_actionContext, [], _controller);

        ActionExecutionDelegate next = () => Task.FromResult(actionExecutedContext);

        // Act
        await _contactsHttpPostFilter.OnActionExecutionAsync(actionExecutingContext, next);

        // Assert
        actionExecutingContext.Result.Should().NotBeNull();
        actionExecutingContext.Result.Should().BeOfType<ViewResult>();
    }

    #endregion

    #region ContactsListActionFilter

    [Fact]
    public void OnActionExecuting_ReturnsName_WhenUnsupportedSearchBy()
    {
        // Arrange
        var actionArguments = new Dictionary<string, object?>
        {
            ["searchBy"] = "InvalidField"
        };
        // OnActionExecuting doesn't use our controller, so we can also pass a mock, e.g Mock.Of<Controller>()
        var context = new ActionExecutingContext(_actionContext, [], actionArguments, _controller);

        // Act
        _contactsListFilter.OnActionExecuting(context);

        // Assert
        context.ActionArguments["searchBy"].Should().Be(nameof(PersonResponse.Name));
    }

    [Fact]
    public void OnActionExecuted_SetsSearchFieldsInViewData_WhenActionExecutes()
    {
        // Arrange
        var context = new ActionExecutedContext(_actionContext, [], _controller);

        // Act
        _contactsListFilter.OnActionExecuted(context);

        // Assert
        var searchFields = _controller.ViewData["SearchFields"]
            .Should().BeOfType<Dictionary<string, string>>().Which;

        searchFields.Should().HaveCount(6);
        searchFields.Should().ContainKey(nameof(PersonResponse.Name));
        searchFields.Should().ContainKey(nameof(PersonResponse.Email));
        searchFields.Should().ContainKey(nameof(PersonResponse.BirthDate));
        searchFields.Should().ContainKey(nameof(PersonResponse.Gender));
        searchFields.Should().ContainKey(nameof(PersonResponse.CountryId));
        searchFields.Should().ContainKey(nameof(PersonResponse.Address));
    }

    [Fact]
    public void OnActionExecuted_SetsSearchAndSortValuesInViewData_WhenArgumentsExist()
    {
        // Arrange
        _httpContext.Items["arguments"] = new Dictionary<string, object?>
        {
            ["searchBy"] = nameof(PersonResponse.Email),
            ["search"] = "Jane",
            ["sortBy"] = nameof(PersonResponse.Name),
            ["sortOrder"] = SortOrder.Ascending
        };
        var context = new ActionExecutedContext(_actionContext, [], _controller);

        // Act
        _contactsListFilter.OnActionExecuted(context);

        // Assert
        _controller.ViewData["SearchBy"].Should().Be(nameof(PersonResponse.Email));
        _controller.ViewData["Search"].Should().Be("Jane");
        _controller.ViewData["SortBy"].Should().Be(nameof(PersonResponse.Name));
        _controller.ViewData["SortOrder"].Should().Be(SortOrder.Ascending.ToString());
    }

    #endregion

    #region ResponseHeaderActionFilter

    [Fact]
    public void OnActionExecuted_AddsResponseHeader()
    {
        // Arrange
        var actionExecutedContext = new ActionExecutedContext(_actionContext, [], _controller);

        // Act
        _responseHeaderFilter.OnActionExecuted(actionExecutedContext);

        // Assert
        _httpContext.Response.Headers["Test-Header"].ToString().Should().Be("TestValue");
    }

    #endregion
}
