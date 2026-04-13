using Noticore.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add Infrastructure services to the container
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

app.Run();
