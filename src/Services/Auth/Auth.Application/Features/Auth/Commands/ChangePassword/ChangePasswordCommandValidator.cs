using FluentValidation;

namespace Auth.Application.Features.Auth.Commands.ChangePassword;

public sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(command => command.UserId).GreaterThan(0);
        RuleFor(command => command.Password.CurrentPassword).NotEmpty();
        RuleFor(command => command.Password.NewPassword).NotEmpty().MinimumLength(10);
        RuleFor(command => command.Password.ConfirmNewPassword)
            .Equal(command => command.Password.NewPassword)
            .WithMessage("A confirmacao deve ser igual a nova senha.");
    }
}
