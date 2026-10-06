using ServiceContract;
using Services;

var builder = WebApplication.CreateBuilder(args);

// IoC / DI container (builder.Services)
builder.Services.AddControllersWithViews();
//builder.Services.Add(new ServiceDescriptor(
//    typeof(ICitiesService),     // 1) service type
//    typeof(CitiesService),      // 2) object that implements the service
//                                // When a new object has to be created by the IoC?
//                                // ServiceLifeTime.Scoped: per browser request
//                                // ServiceLifeTime.Singleton: entire app lifetime
//    ServiceLifetime.Transient   // Transient (used here): per injection
//));

// alternative shorthand version
builder.Services.AddTransient<ICitiesService, CitiesService>();

var app = builder.Build();

app.UseStaticFiles();
app.MapControllers();

app.Run();