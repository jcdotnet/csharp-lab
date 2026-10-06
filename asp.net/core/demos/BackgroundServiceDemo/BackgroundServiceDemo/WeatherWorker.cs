using System.Text.Json;

namespace BackgroundServiceDemo;

public class WeatherWorker(
    ILogger<WeatherWorker> logger,
    IHttpClientFactory httpClientFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const string apiUrl = "https://api.open-meteo.com/v1/forecast";

        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await GetWeatherAsync(apiUrl, stoppingToken);

                // await Task.Delay(60000, stoppingToken); // using PeriodicTimer instead
                if (!await timer.WaitForNextTickAsync(stoppingToken)) break;
            }
        }
        catch (OperationCanceledException)
        {
            if (logger.IsEnabled(LogLevel.Information))
                logger.LogInformation("The weather worker has been cancelled");
        }
    }

    private async Task GetWeatherAsync(string apiUrl, CancellationToken stoppingToken)
    {
        var client = httpClientFactory.CreateClient();

        var url = $"{apiUrl}?latitude=36.72&longitude=-4.42&current=temperature_2m,weather_code";

        var response = await client.GetAsync(url, stoppingToken);
        var responseContent = await response.Content.ReadAsStringAsync(stoppingToken);
        var weatherData = JsonSerializer.Deserialize<WeatherData>(responseContent);

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Weather API status: {StatusCode}", response.StatusCode);
            logger.LogInformation("Weather API response: {ResponseContent}", responseContent);
            logger.LogInformation("Current temperature: {Temperature}°C", weatherData?.Weather.Temperature);
            logger.LogInformation("Current Condition: {Condition}", weatherData?.Weather.Condition);
        }
    }
}