using Microsoft.AspNetCore.Mvc;
using Models;
using ServiceContracts;

namespace WeatherMvcApp.Controllers
{
    public class WeatherController : Controller
    {
        private readonly IWeatherService _weatherService;

        //Create a constructor and inject IWeatherService
        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [Route("/")]
        public IActionResult Index()
        {
            var cities = _weatherService.GetWeatherDetails();
            return View(cities); 
        }

        [Route("weather/{cityCode?}")]
        public IActionResult City(string cityCode)
        {
            if (string.IsNullOrEmpty(cityCode))
            {
                return View();
            }
            CityWeather? city = _weatherService.GetWeatherByCityCode(cityCode);
            return View(city);
        }
    }
}
