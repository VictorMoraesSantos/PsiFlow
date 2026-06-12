using BuildingBlocks.Results;

namespace Auth.Domain.Errors
{
    public static class TenantErrors
    {
        public static Error InvalidId => Error.Failure("TenantId invalido.");
        public static Error NullTenant => Error.Failure("Tenant e obrigatorio.");
        public static Error TenantMismatch => Error.Failure("Tenant nao corresponde ao usuario informado.");
        public static Error NotFound(int id) => Error.NotFound($"Tenant nao encontrado: {id}");
    }
}
