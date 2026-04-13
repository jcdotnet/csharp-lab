using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Noticore.Application.Interfaces;
using Noticore.Infrastructure.Persistence;
using Noticore.Infrastructure.Repositories;

namespace Noticore.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, 
            IConfiguration configuration)
        {
            // Database Configuration
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            // Repositories
            services.AddScoped<INotificationRepository, NotificationRepository>();

            // Simulated Email Service
            services.AddTransient<IEmailService, EmailService>();

            return services;
        }
    }
}
