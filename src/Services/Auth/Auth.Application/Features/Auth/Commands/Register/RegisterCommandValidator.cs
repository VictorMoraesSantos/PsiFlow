using FluentValidation;

namespace Auth.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(command => command.Data.Role).NotEmpty().Must(role => role is "psychologist" or "patient" or "saas_admin");
        RuleFor(command => command.Data.FirstName).NotEmpty().MinimumLength(2).MaximumLength(80);
        RuleFor(command => command.Data.LastName).NotEmpty().MinimumLength(2).MaximumLength(80);
        RuleFor(command => command.Data.Email).NotEmpty().EmailAddress().MaximumLength(254);
        RuleFor(command => command.Data.Password).NotEmpty().MinimumLength(10);
        RuleFor(command => command.Data.ConfirmPassword).Equal(command => command.Data.Password);
        RuleFor(command => command.Data.AcceptedTermsVersion).NotEmpty();
        RuleFor(command => command.Data.AcceptedPrivacyVersion).NotEmpty();
        RuleFor(command => command.Data.Crp)
            .NotEmpty()
            .Matches(@"^\d{2}/\d{4,6}$")
            .When(command => command.Data.Role == "psychologist");
    }
}
