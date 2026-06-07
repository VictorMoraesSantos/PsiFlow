using BuildingBlocks.Results;

namespace Auth.Domain.Errors
{
    public static class ContactErrors
    {
        public static Error InvalidFormat => Error.Failure("Email format is invalid");
    }
}