using BuildingBlocks.Results;

namespace Auth.Domain.Errors
{
    public static class PermissionErrors
    {
        public static Error InvalidId => Error.Failure("Id must be valid.");
        public static Error InvalidClaimType => Error.Failure("Claim type cannot be empty.");
        public static Error InvalidClaimValue => Error.Failure("Claim value cannot be empty.");
        public static Error InvalidGroupKey => Error.Failure("Key cannot be empty.");
        public static Error InvalidGroupName => Error.Failure("Name cannot be empty.");
        public static Error InvalidDescription => Error.Failure("Description cannot be empty.");
        public static Error InvalidKeyGroup => Error.Failure("Key group cannot be empty.");
        public static Error PermissionAlreadyExists => Error.Failure("Permission already exists.");
        public static Error NotFound(int id) => Error.NotFound($"Id: {id} was not found.");
        public static Error NotFound(string claimType, string claimValue) => Error.NotFound($"Permission '{claimType}:{claimValue}' was not found.");
        public static Error Duplicate(string claimType, string claimValue) => Error.Failure($"Permission '{claimType}:{claimValue}' already exists.");
    }
}
