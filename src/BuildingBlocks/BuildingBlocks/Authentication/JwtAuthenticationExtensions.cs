using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;

namespace BuildingBlocks.Authentication
{
    public static class JwtAuthenticationExtensions
    {
        private static readonly ConcurrentDictionary<string, JsonWebKeySet> JwksCache = new();
        private static readonly HttpClient HttpClient = new();

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var jwksUri = jwtSettings["JwksUri"];

            if (string.IsNullOrWhiteSpace(jwksUri))
                throw new InvalidOperationException("JwtSettings:JwksUri is not configured in appsettings.json");

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKeyResolver = (_, _, kid, _) => ResolveSigningKeys(jwksUri, kid),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddAuthorization();

            return services;
        }

        private static IEnumerable<SecurityKey> ResolveSigningKeys(string jwksUri, string? kid)
        {
            var jwks = JwksCache.GetOrAdd(jwksUri, uri => new JsonWebKeySet(HttpClient.GetStringAsync(uri).GetAwaiter().GetResult()));
            return string.IsNullOrWhiteSpace(kid)
                ? jwks.Keys
                : jwks.Keys.Where(key => key.Kid == kid);
        }
    }
}
