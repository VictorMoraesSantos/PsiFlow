using Sessions.Application.Contracts;
using Sessions.Domain.Repositories;
using Sessions.Infrastructure.Persistence.Repositories;
using Sessions.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PsiFlow.Sessions.Infrastructure.Persistence;

namespace Sessions.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSessionsInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<global::PsiFlow.Sessions.Infrastructure.Persistence.SessionsDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("Database")));
            services.AddScoped<ISessionRepository, SessionRepository>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<ISessionWorkflowService, SessionWorkflowService>();
            services.AddHostedService<SessionsDatabaseInitializer>();
            return services;
        }
    }
}
