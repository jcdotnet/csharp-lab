using BackgroundServiceDemo;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddHostedService<WeatherWorker>();

var host = builder.Build();
host.Run();
