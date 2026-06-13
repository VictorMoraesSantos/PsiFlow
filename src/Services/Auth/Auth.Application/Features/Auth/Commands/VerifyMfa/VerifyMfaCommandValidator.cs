using FluentValidation;

namespace Auth.Application.Features.Auth.Commands.VerifyMfa;

public sealed class VerifyMfaCommandValidator : AbstractValidator<VerifyMfaCommand>
{
    public VerifyMfaCommandValidator()
    {
        RuleFor(command => command.UserId).GreaterThan(0);
        RuleFor(command => command.Code.Code).NotEmpty().Matches(@"^\d{6}$");
    }
}
