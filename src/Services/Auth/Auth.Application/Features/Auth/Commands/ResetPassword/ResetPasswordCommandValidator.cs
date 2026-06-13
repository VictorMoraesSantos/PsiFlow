using FluentValidation;

namespace Auth.Application.Features.Auth.Commands.ResetPassword;

public sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(command => command.Data.Email).NotEmpty().EmailAddress();
        RuleFor(command => command.Data.Token).NotEmpty();
        RuleFor(command => command.Data.NewPassword).NotEmpty().MinimumLength(10);
        RuleFor(command => command.Data.ConfirmPassword).Equal(command => command.Data.NewPassword);
    }
}
