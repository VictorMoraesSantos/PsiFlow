using Auth.Application.Settings;
using Microsoft.IdentityModel.Tokens;

namespace Auth.API.Endpoints
{
    public static class JwksEndpoints
    {
        public static IEndpointRouteBuilder MapJwksEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/.well-known/jwks.json", (JwtSettings settings) =>
            {
                var jwk = new JsonWebKey
                {
                    Kty = "oct",
                    Kid = "psiflow-dev",
                    Use = "sig",
                    Alg = SecurityAlgorithms.HmacSha256,
                    K = Base64UrlEncoder.Encode(settings.Key)
                };

                return Results.Json(new { keys = new[] { jwk } });
            }).AllowAnonymous();

            return app;
        }
    }
}
