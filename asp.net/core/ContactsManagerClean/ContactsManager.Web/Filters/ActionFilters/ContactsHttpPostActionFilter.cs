using ContactsManager.Web.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;

namespace ContactsManager.Filters.ActionFilters;

/// <summary>
/// This action filter checks model validation before the action method executes
/// and prepares the countries list when the submitted data is invalid
/// </summary>
public class ContactsHttpPostActionFilter : IAsyncActionFilter
{
    private readonly ICountriesGetterService _countriesService;
    private readonly ILogger<ContactsHttpPostActionFilter> _logger;


    public ContactsHttpPostActionFilter(ILogger<ContactsHttpPostActionFilter> logger, 
        ICountriesGetterService countriesService)
    {
        _logger = logger;
        _countriesService = countriesService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // before logic
        _logger.LogInformation("-----> {FilterName} ---> {MethodName} (before)",
            nameof(ContactsListActionFilter),
            nameof(OnActionExecutionAsync)
        );
        if (context.Controller is ContactsController controller)
        {

            if (!controller.ModelState.IsValid) // (!context.ModelState.IsValid) 
            {
                _logger.LogInformation("-----> {FilterName} ---> {MethodName} (validation logic)",
                    nameof(ContactsListActionFilter),
                    nameof(OnActionExecutionAsync)
                );
                // this happens when client side validations fails (very rarely)
                var countries = await _countriesService.GetCountries();
                controller.ViewBag.Countries = countries.Select(country => new SelectListItem()
                {
                    Text = country.Name,
                    Value = country.Id.ToString()
                });

                // using cllient server validation instead
                //controller.ViewBag.Errors = controller.ModelState.Values.SelectMany(v =>
                //v.Errors).Select(e => e.ErrorMessage).ToList();

                var personRequest = context.ActionArguments["personRequest"];
                // short-curcuit request: returning any type of IActionResult
                context.Result = controller.View(personRequest);
                // not calling next to short-circuit the subsequent action filters & action method
                //await next();
                return; // added return to refactor the if/else code
            }
            else
            {
                // modelstate is valid
                //await next();
            }
        }
        else
        {
            // controller is not ContactsController
            //await next();
        }
        await next(); // call the subsequent action filter or the action method
        _logger.LogInformation("-----> {FilterName} ---> {MethodName} (after)",
            nameof(ContactsListActionFilter),
            nameof(OnActionExecutionAsync)
        );
    }
}
