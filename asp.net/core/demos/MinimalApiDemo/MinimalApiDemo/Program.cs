using MinimalApiDemo.RouteGroups;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", async (HttpContext context) =>
{
    await context.Response.WriteAsync("GET - Minimal API Demo project");
});

app.MapGet("/hello", () => "Hello World!");

var mapGroup = app.MapGroup("/products").ProductsAPI();

app.Run();
