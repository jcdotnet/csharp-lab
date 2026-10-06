using System.Text.Json.Serialization;

namespace BackgroundServiceDemo;

public class WeatherData
{
    // JSON deserialization: Maps "Current" (API property) to "Weather" (model).
    [JsonPropertyName("current")]
    public Weather Weather { get; set; } = new();
}

public class Weather
{
    [JsonPropertyName("temperature_2m")]
    public double Temperature { get; set; }

    [JsonPropertyName("weather_code")]
    public WeatherCondition Condition { get; set; }
}
