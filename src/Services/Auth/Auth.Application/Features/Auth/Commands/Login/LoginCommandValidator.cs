using FluentValidation;

namespace Auth.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(command => command.Credentials.Email).NotEmpty().EmailAddress();
        RuleFor(command => command.Credentials.Password).NotEmpty();
    }
}
