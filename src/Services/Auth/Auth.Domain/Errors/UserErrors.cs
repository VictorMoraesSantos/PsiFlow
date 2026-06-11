using BuildingBlocks.Results;

namespace Auth.Domain.Errors
{
    public static class UserErrors
    {
        public static Error InvalidId => Error.Failure("Id invalido.");
        public static Error NullContact => Error.Failure("Contato e obrigatorio.");
        public static Error InvalidBirthDate => Error.Failure("Data de nascimento invalida.");
        public static Error BirthDateInFuture => Error.Failure("Data de nascimento nao pode ser no futuro.");
        public static Error AlreadyActive => Error.Failure("Usuario ja esta ativo.");
        public static Error AlreadyInactive => Error.Failure("Usuario ja esta inativo.");
        public static Error AlreadyDeleted => Error.Failure("Usuario ja foi excluido.");
        public static Error InvalidCredentials => Error.Failure("Credenciais invalidas.");
        public static Error UserLockedOut => Error.Failure("Conta bloqueada por multiplas tentativas. Tente novamente em alguns minutos.");
        public static Error PasswordTooWeak => Error.Failure("Senha fraca. Deve ter ao menos 10 caracteres, com maiuscula, minuscula, numero e simbolo.");
        public static Error PasswordsDoNotMatch => Error.Failure("Senha e confirmacao nao conferem.");
        public static Error RefreshTokenInvalid => Error.Failure("Refresh token invalido.");
        public static Error RefreshTokenExpired => Error.Failure("Refresh token expirado.");
        public static Error RefreshTokenReused => Error.Failure("Refresh token reutilizado. Sessao sera revogada.");
        public static Error CrpRequired => Error.Failure("CRP e obrigatorio para psicologas.");
        public static Error CrpInvalid => Error.Failure("Formato de CRP invalido. Use NN/NNNNNN.");
        public static Error TenantRequired => Error.Failure("TenantId invalido.");
        public static Error RoleInvalid => Error.Failure("Role invalida. Use 'psychologist', 'patient' ou 'saas_admin'.");
        public static Error MfaNotAllowed => Error.Failure("MFA esta disponivel apenas para psicologas e administradores SaaS.");
        public static Error MfaChallengeNotFound => Error.NotFound("Challenge MFA ativo nao encontrado.");
        public static Error MfaCodeInvalid => Error.Failure("Codigo MFA invalido.");
        public static Error TermsNotAccepted => Error.Failure("Os termos e a politica de privacidade devem ser aceitos.");
        public static Error EmailNotConfirmed => Error.Failure("E-mail ainda nao foi confirmado.");
        public static Error CreateError => Error.Failure("Falha ao criar usuario.");
        public static Error UpdateError => Error.Failure("Falha ao atualizar usuario.");
        public static Error DeleteError => Error.Failure("Falha ao excluir usuario.");
        public static Error ActivateError => Error.Failure("Falha ao ativar usuario.");
        public static Error DeactivateError => Error.Failure("Falha ao desativar usuario.");

        public static Error EmailAlreadyInUse(string email) => Error.Failure($"E-mail {email} ja esta em uso.");
        public static Error NotFound(int id) => Error.NotFound($"Usuario nao encontrado: {id}");
    }
}
