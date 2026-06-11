using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notifications.Application.Contracts;
using Notifications.Domain.Repositories;
using Notifications.Infrastructure.Persistence.Repositories;
using Notifications.Infrastructure.Services;
using PsiFlow.Notifications.Infrastructure.Persistence.Data;

namespace Notifications.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddNotificationsInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<global::PsiFlow.Notifications.Infrastructure.Persistence.Data.NotificationsDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("Database")));
            services.AddScoped<INotificationTemplateRepository, NotificationTemplateRepository>();
            services.AddScoped<INotificationTemplateService, NotificationTemplateService>();
            services.AddScoped<INotificationWorkflowService, NotificationWorkflowService>();
            services.AddHostedService<NotificationsDatabaseInitializer>();
            return services;
        }
    }
}
