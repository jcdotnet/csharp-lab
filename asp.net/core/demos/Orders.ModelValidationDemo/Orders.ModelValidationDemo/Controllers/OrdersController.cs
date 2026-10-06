using Microsoft.AspNetCore.Mvc;
using Orders.ModelValidationDemo.Models;

namespace Orders.ModelValidationDemo.Controllers
{
    public class OrdersController : Controller
    {
        [Route("/order")]
        //binding desired properties of Order class only
        public IActionResult Index([Bind(nameof(Order.Date), 
            nameof(Order.InvoicePrice), 
            nameof(Order.Products))] Order order)
        {
            if (!ModelState.IsValid) // validation errors (as per data annotations)
            {
                string messages = string.Join('\n', ModelState.Values.SelectMany(v 
                    => v.Errors).Select(e => e.ErrorMessage));

                return BadRequest(messages);
            }

            Random random = new Random();
            int randomOrderNumber = random.Next(1, 99999);

            return Json(new { orderNumber = randomOrderNumber }); // HTTP 200
        }
    }
}
