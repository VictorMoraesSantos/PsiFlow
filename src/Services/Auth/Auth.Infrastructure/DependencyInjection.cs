using Auth.Application;
using Auth.Application.Services;
using Auth.Application.Settings;
using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using Auth.Domain.ValueObjects;
using Auth.Infrastructure.Authentication;
using Auth.Infrastructure.Persistence.Data;
using Auth.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using AuthOptions = Auth.Application.Settings.AuthOptions;

namespace Auth.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("Database")));
            services.AddRepositories();
            services.AddIdentityServices(configuration);
            services.AddAuthApplication();
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.PostConfigure<JwtSettings>(settings => ApplyDevelopmentFallbacks(settings, configuration));
            services.Configure<AuthOptions>(configuration.GetSection("Auth"));
            services.PostConfigure<AuthOptions>(options =>
            {
                if (!options.AutoConfirmEmails && IsDevelopmentEnvironment(configuration))
                    options.AutoConfirmEmails = true;
            });
            services.Configure<Auth.Infrastructure.Persistence.Seeds.AuthSeedOptions>(configuration.GetSection("AuthSeed"));
            services.AddSingleton(sp => sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<JwtSettings>>().Value);
            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IConsentRepository, ConsentRepository>();
            services.AddScoped<IMfaChallengeRepository, MfaChallengeRepository>();
            services.AddScoped<IOutboxRepository, OutboxRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IPermissionGroupRepository, PermissionGroupRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
            return services;
        }

        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? new JwtSettings();
            if (string.IsNullOrWhiteSpace(jwtSettings.Issuer)) jwtSettings.Issuer = "psiflow-auth";
            if (string.IsNullOrWhiteSpace(jwtSettings.Audience)) jwtSettings.Audience = "psiflow-api";
            if (string.IsNullOrWhiteSpace(jwtSettings.KeyId)) jwtSettings.KeyId = "psiflow-auth-rsa-1";
            ApplyDevelopmentFallbacks(jwtSettings, configuration);

            services.AddSingleton(jwtSettings);
            services.AddSingleton<Auth.Application.Services.EncryptionService>();
            services.AddSingleton<JwtRsaKeyProvider>();
            services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();
            services.AddAuthorization();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer();

            services.AddIdentity<User, IdentityRole<UserId>>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 10;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            return services;
        }

        private const string DevelopmentEncryptionKey = "dev-only-jwt-encryption-key-replace-in-production-must-be-32-bytes!";

        private static void ApplyDevelopmentFallbacks(JwtSettings settings, IConfiguration configuration)
        {
            if (settings is null) return;
            if (!string.IsNullOrWhiteSpace(settings.EncryptionKey)) return;
            if (!IsDevelopmentEnvironment(configuration)) return;
            settings.EncryptionKey = DevelopmentEncryptionKey;
        }

        private static bool IsDevelopmentEnvironment(IConfiguration configuration)
        {
            var env = configuration["ASPNETCORE_ENVIRONMENT"]
                ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                ?? Environments.Development;
            var result = string.Equals(env, Environments.Development, StringComparison.OrdinalIgnoreCase);
            return result;
        }
    }
}
