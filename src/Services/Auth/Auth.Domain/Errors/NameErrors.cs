using BuildingBlocks.Results;

namespace Auth.Domain.Errors
{
    public static class NameErrors
    {
        public static Error NullName => Error.Failure("Name cannot be null or empty");
    }
}