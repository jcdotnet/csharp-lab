using Microsoft.AspNetCore.Mvc;
using ServiceContract;

namespace DependencyInjection.Controllers
{
    public class HomeController : Controller
    {
        // direct dependency
        // private readonly CitiesService _citiesService; 

        // private readonly ICitiesService _citiesService;

        //public HomeController()
        //{
        //    // direct dependency
        //    _citiesService = new CitiesService(); // BAD PRACTICE!!!
        //}

        // GOOD PRACTICE: Dependency Injection (constructor injection)
        //public HomeController(ICitiesService service)
        //{
        //    _citiesService = service; // object from IoC container
        //}

        // Dependency Injection (method injection)
        [Route("/")]
        public IActionResult Index(
            [FromServices] ICitiesService _citiesService)
        {
            List<string> cities = _citiesService.GetCities();
            return View(cities); // strongly typed view
        }
    }
}
