using Demo.Models;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers
{
    public class UserController : Controller
    {
        [Route("user/register")]
        public IActionResult Register(User user)
        {
            if (!ModelState.IsValid)
            {
                //List<string> errors = new List<string>(); // maybe StringBuilder instead
                //foreach (var value in ModelState.Values) // model property
                //{
                //    foreach (var error in value.Errors) // validation errors on property
                //    {
                //        errors.Add(error.ErrorMessage);
                //    } 
                //    return BadRequest(string.Join('\n', errors));
                //}

                // let's simplify the code (LINQ)
                //List<string> errors = ModelState.Values.SelectMany( value => 
                //    value.Errors.Select(err => err.ErrorMessage)
                //).ToList();
                //return BadRequest(string.Join('\n', errors));

                string errors = string.Join('\n', ModelState.Values.SelectMany(value =>
                    value.Errors.Select(err => err.ErrorMessage)
                ));
                return BadRequest(errors);
            }
            return Content($"{user}");
        }
    }
}
