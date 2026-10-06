var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseRouting();

Dictionary<int, string> countries = new()
{
 { 1, "United States" },
 { 2, "Canada" },
 { 3, "United Kingdom" },
 { 4, "Spain" },
 { 5, "Japan" }
};

#pragma warning disable ASP0014 // Suggest using top level route registrations
app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/countries", async context =>
    {
        foreach (var country in countries)
        {
            await context.Response.WriteAsync($"{country.Key}. {country.Value}\n");
        }
    });

    endpoints.MapGet("/countries/{countryId:int:range(1,100)}", async context =>
    {
        int countryId = Convert.ToInt32(context.Request.RouteValues["countryId"]);

        if (countries.ContainsKey(countryId))
        {
            string countryName = countries[countryId];
            await context.Response.WriteAsync(countryName);
        }
        else
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync("[Country Not Found]");
        }
    });

    endpoints.MapGet("/countries/{countryId:int:min(101)}", async context =>
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("The CountryId should be between 1 and 100");
    });
});
#pragma warning restore ASP0014 // Suggest using top level route registrations

app.Run(async context =>
{
    await context.Response.WriteAsync("No response");
});

app.Run();
