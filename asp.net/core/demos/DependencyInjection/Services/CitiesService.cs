using ServiceContract;

namespace Services
{
    public class CitiesService : ICitiesService
    {
        private readonly List<string> _cities = [
            "Amsterdam",
            "Barcelona",
            "Berlin",
            "Dublin",
            "Madrid",
            "Málaga",
            "Munich",
            "New York",
            "London",
            "Paris",
            "Rome",
            "San Francisco",
            "Tokyo",
            "Vienna",
            "Zurich"
        ];
       
        public List<string> GetCities()
        {
            return _cities;
        }
    }
}
