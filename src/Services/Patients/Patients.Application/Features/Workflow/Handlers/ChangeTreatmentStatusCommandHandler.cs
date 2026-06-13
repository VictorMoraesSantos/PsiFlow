using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using FluentValidation;
using Patients.Application.Contracts;

namespace Patients.Application.Features.Workflow;

public sealed class ChangeTreatmentStatusCommandHandler(IPatientInviteService service, IValidator<ChangeTreatmentStatusCommand> validator) : ICommandHandler<ChangeTreatmentStatusCommand, object>
{
    public async Task<Result<object>> Handle(ChangeTreatmentStatusCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        return validation.IsValid
            ? await service.ChangeTreatmentStatusAsync(command.PatientId, command.TreatmentStatus, command.Reason, command.TenantId, command.UserId, command.CorrelationId, cancellationToken)
            : Result.Failure<object>(Error.Failure(string.Join("; ", validation.Errors.Select(x => x.ErrorMessage))));
    }
}
