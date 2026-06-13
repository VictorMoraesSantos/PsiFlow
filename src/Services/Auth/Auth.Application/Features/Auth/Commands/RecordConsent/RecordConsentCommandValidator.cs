using FluentValidation;

namespace Auth.Application.Features.Auth.Commands.RecordConsent;

public sealed class RecordConsentCommandValidator : AbstractValidator<RecordConsentCommand>
{
    public RecordConsentCommandValidator()
    {
        RuleFor(command => command.UserId).GreaterThan(0);
        RuleFor(command => command.Consent.TermsVersion).NotEmpty();
        RuleFor(command => command.Consent.PrivacyVersion).NotEmpty();
    }
}
