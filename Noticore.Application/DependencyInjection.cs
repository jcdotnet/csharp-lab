using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Noticore.Application
{
    public static class DependencyInjection
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddApplication()
            {
                services.AddMediatR(cfg => {
                    cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
                });

                // Fluent validation
                services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

                return services;
            }
        }
    }
}
