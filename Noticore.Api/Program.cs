using Noticore.Api.Middleware;
using Noticore.Application;
using Noticore.Application.Interfaces;
using Noticore.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add Infrastructure services to the container
builder.Services.AddInfrastructure(builder.Configuration);

// Add Application services to the container
builder.Services.AddApplication();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
