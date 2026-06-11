using FluentValidation;

namespace Patients.Application.Features.Workflow;

public sealed class ChangeTreatmentStatusCommandValidator : AbstractValidator<ChangeTreatmentStatusCommand>
{
    public ChangeTreatmentStatusCommandValidator() => RuleFor(x => x.TreatmentStatus).Must(x => PatientStatus.AllowedTreatmentStatuses.Contains(x));
}
