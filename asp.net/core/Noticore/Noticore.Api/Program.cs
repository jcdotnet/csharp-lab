using Microsoft.OpenApi;
using Noticore.Api.Middleware;
using Noticore.Application;
using Noticore.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add Infrastructure services to the container
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);

// Add Application services to the container
builder.Services.AddApplication();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Noticore API",
        Version = "v1",
        Description = "API for managing notifications with Polly and MailKit"
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Noticore API V1");
    });
}

app.UseExceptionHandler();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
