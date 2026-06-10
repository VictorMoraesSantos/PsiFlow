using FluentValidation;

namespace ClinicalRecords.Application.Features.MedicalRecords.Commands.Create;

public sealed class CreateMedicalRecordCommandValidator : AbstractValidator<CreateMedicalRecordCommand>
{
    public CreateMedicalRecordCommandValidator()
    {
        RuleFor(command => command.MedicalRecord.TenantId).GreaterThan(0);
        RuleFor(command => command.MedicalRecord.PatientId).GreaterThan(0);
        RuleFor(command => command.MedicalRecord.Name).NotEmpty().MaximumLength(200);
    }
}
