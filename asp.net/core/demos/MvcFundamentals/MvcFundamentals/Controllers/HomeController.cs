using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers
{
    public class HomeController : Controller
    {
        [Route("/")]
        public string Index()
        {
            return "Hello, World!";
        }

        [Route("about")]
        public string About()
        {
            return "Software Engineer";
        }

    }
}
