using MinimalApiDemo.EndPointFilters;
using MinimalApiDemo.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace MinimalApiDemo.RouteGroups
{
    public static class ProductsRouteGroup
    {
        // products: to use services and repositories instead
        private static readonly List<Product> products = [ 
            new Product(){ Id = 1, Name = "Laptop"},
            new Product(){ Id = 2, Name = "Desktop"},
            new Product(){ Id = 3, Name = "Phone"},
            new Product(){ Id = 4, Name = "Smart TV"},
            new Product(){ Id = 5, Name = "Tablet"},
        ];

        public static RouteGroupBuilder ProductsAPI(this RouteGroupBuilder mapGroup)
        {    
            mapGroup.MapGet("/", async (HttpContext context) =>
            {
                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(products)
                );
            });

            // Minimal API supports route parameters
            mapGroup.MapGet("/{id:int}", async (HttpContext context, int id) =>
            {
                Product? product = products.FirstOrDefault(p => p.Id == id);
                if (product == null)
                {
                    context.Response.StatusCode = 400; // bad request
                    await context.Response.WriteAsync("Invalid ID");
                    return;
                }
                //await context.Response.WriteAsync(product.ToString());
                // returning JSON instead of plain text
                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(product)
                );
            });

            // Minimal API supports model binding (via URL qs, JSON/XML format only)
            mapGroup.MapPost("/", async (HttpContext context, Product product) =>
            {
                products.Add(product);
                await context.Response.WriteAsync($"Products: {product.Name} added");
            })
            .AddEndpointFilter<CustomEndpointFilter>()
            .AddEndpointFilter(async (
                EndpointFilterInvocationContext context,
                EndpointFilterDelegate next) =>
            {
                // before logic goes here
                var product = context.Arguments.OfType<Product>().FirstOrDefault();
                if (product == null) return Results.BadRequest("Invalid product");
                var validationContext = new ValidationContext(product);
                List<ValidationResult> errors = new List<ValidationResult>();
                bool valid = Validator.TryValidateObject(product, validationContext, errors, true);

                if (!valid)
                {
                    return Results.BadRequest(errors.FirstOrDefault()?.ErrorMessage);
                }

                var result = await next(context); // calls subsquent filter or endpoint

                // after logic goes here
                return result;
            });

            // model binding: we can explicitly add the FromBody, FromQuery attributes
            // e.g. [FromBody] Product product if the data comes from the request body
            mapGroup.MapPut("/{id}", async (HttpContext context, int id, Product product) =>
            {
                Product? productFromList = products.FirstOrDefault(p => p.Id == id);
                if (productFromList == null)
                {
                    context.Response.StatusCode = 400; // bad request
                    await context.Response.WriteAsync("Invalid ID");
                    return;
                }
                productFromList.Name = product.Name;
                await context.Response.WriteAsync($"Product with Id {product.Id} updated");
            });

            mapGroup.MapDelete("/{id}", (HttpContext context, int id) =>
            {
                Product? productFromList = products.FirstOrDefault(p => p.Id == id);
                if (productFromList == null)
                {
                    //context.Response.StatusCode = 400; // bad request
                    //await context.Response.WriteAsync("Invalid ID");
                    //return;
                    // we can return implementations of IResult in Minimal APIs 
                    return Results.BadRequest(new { message = "Invalid ID" });
                }
                products.Remove(productFromList);
                //await context.Response.WriteAsync($"Product deleted");
                return Results.Ok("Product Deleted");
            });

            return mapGroup;
        }
    }
}