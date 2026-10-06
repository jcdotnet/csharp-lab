using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Demo.Controllers
{
    public class EmployeeController : Controller
    {

        [Route("employees")]
        public string Index()
        {
            return "All employees go here";
        }

        // ContentResult can represent any type of data, based on the specified MIME type
        // https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.contentresult?view=aspnetcore-9.0
        [Route("employees/content-demo")]
        public ContentResult ContentResultDemo()
        {
            // return new ContentResult() { Content = "Hello from the Employee controller" };
            // return Content("Hello from the Employee controller"); // simplified
            return Content("<h1>Hello from the Employee controller</h1>", "text/html"); // HTML result
        }

        // ActionResult (controller result)
        // https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.actionresult?view=aspnetcore-9.0
        // JsonResult
        // https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.jsonresult?view=aspnetcore-9.0
        [Route("employees/json-demo")]
        public JsonResult JsonResultDemo()
        {
            // Employee employee = new Employee() { Id = Guid.NewGuid(), FirstName="John", LastName="Doe", Age=40 };
            Employee employee = new Employee() { Id = 101, FirstName = "John", LastName = "Doe", Age = 40 };
            return new JsonResult(employee);
        }

        // RedirectToActionResult
        // https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.redirecttoactionresult?view=aspnetcore-9.0
        [Route("employees/redirect")]
        public IActionResult Redirect() 
        {
            // return new RedirectToActionResult("Index", "Home", new { }); // redirects to home
            return new RedirectToActionResult("Index", "Employee", new { }); // redirects to employees
        }

        // IActionResult
        // https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult?view=aspnetcore-9.0
        // StatusCodeResult
        // https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.statuscoderesult?view=aspnetcore-9.0
        [Route("employees/{id:int}")]
        public IActionResult FindEmployee(Employee employee)
        {
            int? empId = employee.Id;
            if (empId < 0 || empId > 1000)
            {
                // Response.StatusCode = 400;
                return BadRequest("Employee id is not valid");
            }
            return Content($"Employee with id {empId} goes here"); // simplified  
        }
    }
}