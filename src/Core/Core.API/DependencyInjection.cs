using Core.API.Middleware;
using Microsoft.AspNetCore.Mvc;

namespace Core.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCoreApi(this IServiceCollection services)
        {
            services.AddProblemDetails();
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddCors();
            return services;
        }

        public static IServiceCollection AddGlobalExceptionHandler(this IServiceCollection services)
        {
            services.AddProblemDetails();
            services.AddExceptionHandler<GlobalExceptionHandler>();
            return services;
        }

        public static WebApplication UseCoreApi(this WebApplication app)
        {
            app.UseExceptionHandler();
            return app;
        }

        public static WebApplication UseGlobalExceptionHandler(this WebApplication app)
        {
            app.UseExceptionHandler();
            return app;
        }
    }
}
