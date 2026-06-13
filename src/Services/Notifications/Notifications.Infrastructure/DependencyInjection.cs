using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notifications.Application.Email;
using Notifications.Domain.Repositories;
using Notifications.Infrastructure.Email;
using Notifications.Infrastructure.Persistence.Repositories;
using Notifications.Infrastructure.Workers;
using PsiFlow.Notifications.Infrastructure.Persistence.Data;

namespace Notifications.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddNotificationsInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<global::PsiFlow.Notifications.Infrastructure.Persistence.Data.NotificationsDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("Database")));
            services.AddScoped<INotificationTemplateRepository, NotificationTemplateRepository>();
            services.AddScoped<INotificationTemplateVersionRepository, NotificationTemplateVersionRepository>();
            services.AddScoped<INotificationLogRepository, NotificationLogRepository>();
            services.AddScoped<IScheduledNotificationRepository, ScheduledNotificationRepository>();
            services.AddHostedService<NotificationsDatabaseInitializer>();
            services.AddHostedService<ScheduledNotificationWorker>();

            var provider = (configuration["EmailProvider"] ?? "fake").Trim().ToLowerInvariant();
            switch (provider)
            {
                case "resend":
                    services.AddSingleton<IEmailProvider, ResendEmailProvider>();
                    break;
                default:
                    services.AddSingleton<IEmailProvider, FakeEmailProvider>();
                    break;
            }

            return services;
        }
    }
}
