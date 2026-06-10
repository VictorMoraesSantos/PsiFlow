using BuildingBlocks.Results;

namespace Auth.Domain.Errors
{
    public static class ContactErrors
    {
        public static Error EmailRequired => Error.Failure("E-mail e obrigatorio.");
        public static Error InvalidFormat => Error.Failure("Formato de e-mail invalido.");
        public static Error EmailTooLong => Error.Failure("E-mail deve ter no maximo 254 caracteres.");
        public static Error InvalidPhone => Error.Failure("Telefone em formato invalido. Use E.164 (ex: +5511999999999).");
    }
}
