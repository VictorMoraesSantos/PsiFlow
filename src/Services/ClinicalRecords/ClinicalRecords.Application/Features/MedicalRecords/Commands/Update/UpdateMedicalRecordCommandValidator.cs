using FluentValidation;

namespace ClinicalRecords.Application.Features.MedicalRecords.Commands.Update;

public sealed class UpdateMedicalRecordCommandValidator : AbstractValidator<UpdateMedicalRecordCommand>
{
    public UpdateMedicalRecordCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
        RuleFor(command => command.MedicalRecord.TenantId).GreaterThan(0);
        RuleFor(command => command.MedicalRecord.PatientId).GreaterThan(0);
        RuleFor(command => command.MedicalRecord.Name).NotEmpty().MaximumLength(200);
    }
}
