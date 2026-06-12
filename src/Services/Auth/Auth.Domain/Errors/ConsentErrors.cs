using BuildingBlocks.Results;

namespace Auth.Domain.Errors
{
    public static class ConsentErrors
    {
        public static Error InvalidId => Error.Failure("Id must be valid.");
        public static Error InvalidUserId => Error.Failure("UserId must be valid.");
    }
}
