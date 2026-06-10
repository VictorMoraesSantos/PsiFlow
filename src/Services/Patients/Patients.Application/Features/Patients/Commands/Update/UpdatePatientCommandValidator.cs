using FluentValidation;

namespace Patients.Application.Features.Patients.Commands.Update;

public sealed class UpdatePatientCommandValidator : AbstractValidator<UpdatePatientCommand>
{
    public UpdatePatientCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
        RuleFor(command => command.Patient.TenantId).GreaterThan(0);
        RuleFor(command => command.Patient.FullName).NotEmpty().MinimumLength(2).MaximumLength(160);
        RuleFor(command => command.Patient.Email).NotEmpty().EmailAddress();
    }
}
