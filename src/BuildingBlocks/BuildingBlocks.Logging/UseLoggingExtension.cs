using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace BuildingBlocks.Logging
{
    public static class UseLoggingExtension
    {
        public static IServiceCollection AddLogging(this IServiceCollection services)
        {
            services.AddLogging(config =>
            {
                config.ClearProviders();
                config.AddSerilog(new LoggerConfiguration()
                    .ReadFrom.Configuration(services.BuildServiceProvider().GetRequiredService<IConfiguration>())
                    .Enrich.FromLogContext()
                    .CreateLogger());
            });
            return services;
        }

        public static WebApplication UseLogging(this WebApplication app)
        {
            app.UseSerilogRequestLogging();
            return app;
        }
    }
}
