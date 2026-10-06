using Microsoft.AspNetCore.Mvc;

namespace MvcDemo.Controllers
{
    public class ProductController : Controller
    {
        [Route("products")]
        public IActionResult Index()
        {
            return View(); // first Views/Product/Index.cshtml, then Views/Shared/Index.cshtml
        }
    }
}
