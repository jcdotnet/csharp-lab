using ServiceContracts;
using Models;

namespace Services
{
    public class WeatherService : IWeatherService
    {
        private readonly List<CityWeather> _cities;

        public WeatherService()
        {
            _cities = [
                new() { Code = "LDN", Name = "London", DateAndTime = Convert.ToDateTime("2030-01-01 8:00"), Temperature = 33 },
                new() { Code = "NYC", Name = "New York", DateAndTime = Convert.ToDateTime("2030-01-01 3:00"), Temperature = 60 },
                new() { Code = "PAR", Name = "Paris", DateAndTime = Convert.ToDateTime("2030-01-01 9:00"), Temperature = 82 }
            ];
        }

        public CityWeather? GetWeatherByCityCode(string cityCode)
        {
            // FirstOrDefault: city is null if the city code does not exist
            return _cities.Where(temp => temp.Code == cityCode).FirstOrDefault();
        }

        public List<CityWeather> GetWeatherDetails()
        {
            return _cities;
        }
    }
}
