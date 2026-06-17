using Auth.Application.Services;

namespace Auth.API.Endpoints
{
    public static class JwksEndpoints
    {
        public static IEndpointRouteBuilder MapJwksEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/.well-known/jwks.json", (JwtRsaKeyProvider keyProvider) =>
            {
                var body = new { keys = keyProvider.PublicKeys };
                var response = Results.Json(body);
                return response;
            }).AllowAnonymous();

            return app;
        }
    }
}
