using BuildingBlocks.Results;

namespace Auth.Domain.Errors
{
    public static class NameErrors
    {
        public static Error NullName => Error.Failure("Nome nao pode ser vazio.");
        public static Error TooShort => Error.Failure("Nome deve ter ao menos 2 caracteres.");
        public static Error TooLong => Error.Failure("Nome deve ter no maximo 80 caracteres.");
    }
}
