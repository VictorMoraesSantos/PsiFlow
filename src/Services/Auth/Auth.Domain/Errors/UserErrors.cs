using BuildingBlocks.Results;

namespace Auth.Domain.Errors
{
    public static class UserErrors
    {
        public static Error InvalidName => Error.Failure("Name is required");
        public static Error InvalidId => Error.Failure("Id must be invalid");
        public static Error NullContact => Error.Failure("Contact is required");
        public static Error InvalidBirthDate => Error.Failure("Birth date is invalid");
        public static Error BirthDateInFuture => Error.Failure("Birth date cannot be in the future");
        public static Error AlreadyActive => Error.Failure("User is already active");
        public static Error AlreadyInactive => Error.Failure("User is already inactive");
        public static Error AlreadyDeleted => Error.Failure("User is already deleted");
        public static Error InvalidCredentials => Error.Failure("Invalid credentials");
        public static Error RefreshTokenInvalid => Error.Failure("Refresh token is invalid");
        public static Error RefreshTokenExpired => Error.Failure("Refresh token has expired");
        public static Error CreateError => Error.Problem("Failed to create user");
        public static Error UpdateError => Error.Problem("Failed to update user");
        public static Error DeleteError => Error.Problem("Failed to delete user");
        public static Error ActivateError => Error.Problem("Failed to activate user");
        public static Error DeactivateError => Error.Problem("Failed to deactivate user");
        public static Error UpdateLastLoginError => Error.Problem("Failed to update user's last login");

        public static Error EmailAlreadyInUse(string email)
        {
            return Error.Failure($"Email {email} is already in use");
        }

        public static Error UsernameAlreadyInUse(string username)
        {
            return Error.Failure($"Username {username} is already in use");
        }

        public static Error NotFound(int id)
        {
            return Error.NotFound($"User with ID {id} was not found");
        }
    }
}