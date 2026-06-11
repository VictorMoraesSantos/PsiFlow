using Core.API.Middleware;

namespace Core.API
{
    public static class DependencyInjection
    {
        private const string LocalDevelopmentCorsPolicy = "LocalDevelopmentCors";

        public static IServiceCollection AddCoreApi(this IServiceCollection services)
        {
            services.AddProblemDetails();
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddCors(options =>
            {
                options.AddPolicy(LocalDevelopmentCorsPolicy, policy =>
                {
                    policy
                        .SetIsOriginAllowed(origin =>
                        {
                            if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri)) return false;
                            return uri.Scheme == Uri.UriSchemeHttp && uri.Port == 5173;
                        })
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
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
            app.UseCors(LocalDevelopmentCorsPolicy);
            return app;
        }

        public static WebApplication UseGlobalExceptionHandler(this WebApplication app)
        {
            app.UseExceptionHandler();
            return app;
        }
    }
}
