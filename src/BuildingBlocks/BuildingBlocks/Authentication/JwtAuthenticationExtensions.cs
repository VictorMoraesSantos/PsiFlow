using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;

namespace BuildingBlocks.Authentication
{
    public static class JwtAuthenticationExtensions
    {
        private static readonly HttpClient HttpClient = new();
        private static readonly ConcurrentDictionary<string, JwksCacheEntry> JwksCache = new();
        private static readonly TimeSpan JwksRefreshInterval = TimeSpan.FromMinutes(5);

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
                        NameClaimType = System.Security.Claims.ClaimTypes.NameIdentifier,
                        RoleClaimType = System.Security.Claims.ClaimTypes.Role,
                        ClockSkew = TimeSpan.FromSeconds(30)
                    };
                });

            services.AddAuthorization();

            return services;
        }

        private static IEnumerable<SecurityKey> ResolveSigningKeys(string jwksUri, string? kid)
        {
            var entry = JwksCache.GetOrAdd(jwksUri, uri => FetchJwks(uri));

            if (DateTimeOffset.UtcNow - entry.FetchedAt > JwksRefreshInterval || !KeyMatches(entry, kid))
            {
                try
                {
                    var refreshed = FetchJwks(jwksUri);
                    JwksCache[jwksUri] = refreshed;
                    entry = refreshed;
                }
                catch
                {
                    // mantem cache anterior em caso de falha temporaria
                }
            }

            return string.IsNullOrWhiteSpace(kid)
                ? entry.Keys.Keys
                : entry.Keys.Keys.Where(key => key.Kid == kid);
        }

        private static bool KeyMatches(JwksCacheEntry entry, string? kid)
        {
            if (string.IsNullOrWhiteSpace(kid)) return entry.Keys.Keys.Count > 0;
            return entry.Keys.Keys.Any(key => key.Kid == kid);
        }

        private static JwksCacheEntry FetchJwks(string uri)
        {
            var payload = HttpClient.GetStringAsync(uri).GetAwaiter().GetResult();
            return new JwksCacheEntry(new JsonWebKeySet(payload), DateTimeOffset.UtcNow);
        }

        private sealed record JwksCacheEntry(JsonWebKeySet Keys, DateTimeOffset FetchedAt);
    }
}
