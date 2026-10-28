using ContactsManager.Core.DTO;
using ContactsManager.Web.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.Filters.ActionFilters;

/// <summary>
/// This action filter validates the search field for the contacts list BEFORE the action method executes
/// and prepares search and sorting values for the view AFTER the action method executes
/// </summary>
public class ContactsListActionFilter : IActionFilter
{
    private readonly ILogger<ContactsListActionFilter> _logger;

    public ContactsListActionFilter(ILogger<ContactsListActionFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        _logger.LogInformation("-----> {FilterName} ---> {MethodName} (after)",
            nameof(ContactsListActionFilter),
            nameof(OnActionExecuted)
        );

        var contactsController = (ContactsController)context.Controller;
        contactsController.ViewData["SearchFields"] = new Dictionary<string, string>()
        {
            { nameof(PersonResponse.Name), "Name" },
            { nameof(PersonResponse.Email), "Email" },
            { nameof(PersonResponse.BirthDate), "Birth Date" },
            { nameof(PersonResponse.Gender), "Gender" },
            { nameof(PersonResponse.CountryId), "Country" },
            { nameof(PersonResponse.Address), "Address" }
        };

        var args = (IDictionary<string, object?>?)context.HttpContext.Items["arguments"];
        if (args != null)
        {
            if (args.TryGetValue("searchBy", out object? value)) // args.ContainsKey
                contactsController.ViewData["SearchBy"] = value;
            if (args.TryGetValue("search", out object? value2)) // args.ContainsKey
                contactsController.ViewData["Search"] = value2;
            if (args.TryGetValue("sortBy", out object? value3)) // args.ContainsKey
                contactsController.ViewData["SortBy"] = value3;
            if (args.TryGetValue("sortOrder", out object? value4)) // args.ContainsKey
                contactsController.ViewData["SortOrder"] = Convert.ToString(value4);
        } 
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        _logger.LogInformation("-----> {FilterName} ---> {MethodName} (before)",
            nameof(ContactsListActionFilter),
            nameof(OnActionExecuting)
        );

        // Fall back to Name when the requested search field is not supported
        if (context.ActionArguments.TryGetValue("searchBy", out object? value))
        {
            var searchByOptions = new List<string>() {
                nameof(PersonResponse.Name),
                nameof(PersonResponse.Email),
                nameof(PersonResponse.BirthDate),
                nameof(PersonResponse.Gender),
                nameof(PersonResponse.CountryId),
                nameof(PersonResponse.Address)
            };
            if (searchByOptions.Any(temp => temp == value as string) == false)
            {
                _logger.LogInformation("searchBy actual value {value}", value);
                context.ActionArguments["searchBy"] = nameof(PersonResponse.Name);
                _logger.LogInformation("searching by {value}", nameof(PersonResponse.Name));
            }
        }

        // passing data to OnActionExecuted
        context.HttpContext.Items["arguments"] = context.ActionArguments;
    }
}
