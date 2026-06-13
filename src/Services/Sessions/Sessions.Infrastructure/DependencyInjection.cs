using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PsiFlow.Sessions.Infrastructure.Persistence.Data;
using Sessions.Application.Contracts;
using Sessions.Domain.Repositories;
using Sessions.Infrastructure.Notifications;
using Sessions.Infrastructure.Persistence.Repositories;

namespace Sessions.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSessionsInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<global::PsiFlow.Sessions.Infrastructure.Persistence.Data.SessionsDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("Database")));
            services.AddScoped<ISessionRepository, SessionRepository>();
            services.AddScoped<ISessionStatusHistoryRepository, SessionStatusHistoryRepository>();
            services.AddScoped<IManualPaymentRepository, ManualPaymentRepository>();
            services.AddScoped<IReceiptRepository, ReceiptRepository>();
            services.AddHttpClient<IReceiptNotificationProvider, HttpReceiptNotificationProvider>();
            services.AddHostedService<SessionsDatabaseInitializer>();
            return services;
        }
    }
}
