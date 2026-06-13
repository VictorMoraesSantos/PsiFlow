using Auth.Application.Services;
using Microsoft.IdentityModel.Tokens;

namespace Auth.API.Endpoints
{
    public static class JwksEndpoints
    {
        public static IEndpointRouteBuilder MapJwksEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/.well-known/jwks.json", (JwtRsaKeyProvider keyProvider) =>
            {
                var jwk = keyProvider.PublicJwk;
                jwk.Use = "sig";
                jwk.Alg = SecurityAlgorithms.RsaSha256;

                return Results.Json(new { keys = new[] { jwk } });
            }).AllowAnonymous();

            return app;
        }
    }
}
