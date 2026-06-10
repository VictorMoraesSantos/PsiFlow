using FluentValidation;

namespace Patients.Application.Features.Patients.Commands.Create;

public sealed class CreatePatientCommandValidator : AbstractValidator<CreatePatientCommand>
{
    public CreatePatientCommandValidator()
    {
        RuleFor(command => command.Patient.TenantId).GreaterThan(0);
        RuleFor(command => command.Patient.FullName).NotEmpty().MinimumLength(2).MaximumLength(160);
        RuleFor(command => command.Patient.Email).NotEmpty().EmailAddress();
        RuleFor(command => command.Patient.BirthDate)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .When(command => command.Patient.BirthDate.HasValue);
    }
}
