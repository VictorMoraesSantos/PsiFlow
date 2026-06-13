using Auth.Application.Services;

namespace Auth.API.Endpoints
{
    public static class JwksEndpoints
    {
        public static IEndpointRouteBuilder MapJwksEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/.well-known/jwks.json", (JwtRsaKeyProvider keyProvider) =>
            {
                return Results.Json(new { keys = keyProvider.PublicKeys });
            }).AllowAnonymous();

            return app;
        }
    }
}
