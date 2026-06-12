using BuildingBlocks.Results;

namespace Auth.Domain.Errors
{
    public static class MfaChallengeErrors
    {
        public static Error InvalidId => Error.Failure("Id invalido.");
        public static Error NullUserId => Error.Failure("UserId e obrigatorio.");
        public static Error NullTenantId => Error.Failure("TenantId e obrigatorio.");
        public static Error SecretRequired => Error.Failure("Secret criptografado do MFA e obrigatorio.");
        public static Error SecretTooShort => Error.Failure("Secret do MFA deve ter ao menos 16 caracteres.");
        public static Error ChallengeNotFound => Error.NotFound("Challenge MFA ativo nao encontrado.");
        public static Error ChallengeExpired => Error.Failure("Challenge MFA expirado. Solicite um novo codigo.");
        public static Error ChallengeAlreadyConfirmed => Error.Failure("Challenge MFA ja foi confirmado.");
        public static Error ChallengeNotConfirmed => Error.Failure("Challenge MFA ainda nao foi confirmado.");
        public static Error UserMismatch => Error.Failure("Challenge MFA nao pertence ao usuario informado.");
        public static Error CreateError => Error.Failure("Falha ao criar challenge MFA.");
        public static Error ConfirmError => Error.Failure("Falha ao confirmar challenge MFA.");
        public static Error NotFound(int userId) => Error.NotFound($"Challenge MFA ativo nao encontrado para o usuario: {userId}");
    }
}
