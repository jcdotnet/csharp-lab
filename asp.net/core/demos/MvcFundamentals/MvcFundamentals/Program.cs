var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddTransient<HomeController>();
// we can let asp.net core detects them and inject them as services
builder.Services.AddControllers(); 

var app = builder.Build();

// middleware chain
// middleware 1 (e.g. this could enable HTTPS redirection)
app.Use(async (HttpContext context, RequestDelegate next) => {
    Console.WriteLine("Running middleware 1");
    // await context.Response.WriteAsync("Running middleware 1\n");
    await next(context);
});
// middleware 2 (e.g. this could enable auth)
app.Use(async (context, next) => {
    Console.WriteLine("Running middleware 2");
    // await context.Response.WriteAsync("Running middleware 2\n");
    await next(context);
});

// middleware 3 (e.g. another component added to the pipeline)
app.Use(async (context, next) => {
    Console.WriteLine("Running middleware 2");
    // await context.Response.WriteAsync("Running middleware 2\n");
    // not a terminal middleware, so we need to invoke the next one
    await next(); 
});

app.MapFallback(async (context) => {
    await context.Response.WriteAsync($"Page Not Found at { context.Request.Path}");
});

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/static-files?view=aspnetcore-9.0
app.UseStaticFiles();

app.MapControllers();

app.Run();
