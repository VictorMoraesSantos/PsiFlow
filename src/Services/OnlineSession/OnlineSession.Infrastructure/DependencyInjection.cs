using OnlineSession.Application.Contracts;
using OnlineSession.Domain.Repositories;
using OnlineSession.Infrastructure.Persistence.Repositories;
using OnlineSession.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PsiFlow.OnlineSession.Infrastructure.Persistence.Data;

namespace OnlineSession.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddOnlineSessionInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<global::PsiFlow.OnlineSession.Infrastructure.Persistence.Data.OnlineSessionDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("Database")));
            services.AddScoped<IVideoRoomRepository, VideoRoomRepository>();
            services.AddScoped<IVideoRoomService, VideoRoomService>();
            services.AddScoped<IOnlineSessionService, OnlineSessionService>();
            services.AddHostedService<OnlineSessionDatabaseInitializer>();
            return services;
        }
    }
}
