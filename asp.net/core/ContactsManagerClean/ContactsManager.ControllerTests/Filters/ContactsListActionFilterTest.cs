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

public class ContactsListActionFilterTest
{
    private readonly Mock<ILogger<ContactsListActionFilter>> _loggerMock = new();

    private readonly ContactsController _controller = new(
        Mock.Of<IContactsAdderService>(),
        Mock.Of<IContactsGetterService>(),
        Mock.Of<IContactsUpdaterService>(),
        Mock.Of<IContactsSorterService>(),
        Mock.Of<IContactsDeleterService>(),
        Mock.Of<ICountriesGetterService>()
    );
    private readonly DefaultHttpContext _httpContext = new();
    private readonly ActionContext _actionContext;

    private readonly ContactsListActionFilter _filter;

    public ContactsListActionFilterTest()
    {
        _actionContext = new ActionContext(_httpContext, new RouteData(), new ActionDescriptor());

        _filter = new ContactsListActionFilter(_loggerMock.Object);
    }

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
        _filter.OnActionExecuting(context);

        // Assert
        context.ActionArguments["searchBy"].Should().Be(nameof(PersonResponse.Name));
    }

    [Fact]
    public void OnActionExecuted_SetsSearchFieldsInViewData_WhenActionExecutes()
    {
        // Arrange
        var context = new ActionExecutedContext(_actionContext, [], _controller);

        // ACt
        _filter.OnActionExecuted(context);

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
        _filter.OnActionExecuted(context);

        // Assert
        _controller.ViewData["SearchBy"].Should().Be(nameof(PersonResponse.Email));
        _controller.ViewData["Search"].Should().Be("Jane");
        _controller.ViewData["SortBy"].Should().Be(nameof(PersonResponse.Name));
        _controller.ViewData["SortOrder"].Should().Be(SortOrder.Ascending.ToString());


    }
}
