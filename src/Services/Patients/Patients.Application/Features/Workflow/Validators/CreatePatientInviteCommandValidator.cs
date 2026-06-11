using FluentValidation;

namespace Patients.Application.Features.Workflow;

public sealed class CreatePatientInviteCommandValidator : AbstractValidator<CreatePatientInviteCommand>
{
    public CreatePatientInviteCommandValidator() => RuleFor(x => x.Email).NotEmpty().EmailAddress();
}
