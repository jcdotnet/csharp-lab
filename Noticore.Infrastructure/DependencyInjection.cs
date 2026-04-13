using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Noticore.Application.Interfaces;
using Noticore.Infrastructure.Persistence;
using Noticore.Infrastructure.Repositories;
using Noticore.Infrastructure.Services;

namespace Noticore.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, 
            IConfiguration configuration, IHostEnvironment environment)
        {
            // Database Configuration
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            // Repositories
            services.AddScoped<INotificationRepository, NotificationRepository>();

            // Email Service
            if (environment.IsDevelopment())
            {
                services.AddTransient<IEmailService, FakeEmailService>();
            }
            else
            {
                services.AddTransient<IEmailService, SmtpEmailService>();
            }
            
            return services;
        }
    }
}
