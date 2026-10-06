using Microsoft.AspNetCore.Mvc;
using Models;

namespace WeatherMvcApp.ViewComponents
{
    public class CityViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(CityWeather city)
        {
            ViewBag.CityCssClass = GetCssClass(city.Temperature);
            return View(city);
        }

        private string GetCssClass(int temperature)
        {
            return temperature switch
            {
                (< 44) => "blue-back",
                (>= 44) and (< 75) => "green-back",
                (>= 75) => "orange-back"
            };
        }
    }
}
