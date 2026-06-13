using BuildingBlocks.Results;

namespace Auth.Domain.Errors
{
    public static class RefreshTokenErrors
    {
        public static Error InvalidId => Error.Failure("Refresh token id invalido.");
        public static Error Reused => Error.Failure("Refresh token reutilizado. Sessao sera revogada.");
    }
}
