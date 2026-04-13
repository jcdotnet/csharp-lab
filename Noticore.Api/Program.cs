using Noticore.Application;
using Noticore.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add Infrastructure services to the container
builder.Services.AddInfrastructure(builder.Configuration);

// Add Application services to the container
builder.Services.AddApplication();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
