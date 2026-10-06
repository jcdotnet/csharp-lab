namespace MinimalApiDemo.EndPointFilters
{
    public class CustomEndpointFilter : IEndpointFilter
    {
        private readonly ILogger<CustomEndpointFilter> _logger;
        public CustomEndpointFilter(ILogger<CustomEndpointFilter> logger)
        {
            _logger = logger;
        }

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, 
            EndpointFilterDelegate next)
        {
            // before logic goes here
            _logger.LogInformation(" ---> Custom Endpoint Filter ---> Before logic");

            var result = await next(context); // calls subsquent filter or endpoint

            // after logic goes here
            _logger.LogInformation(" ---> Custom Endpoint Filter ---> After logic");

            return result;
        }
    }
}
