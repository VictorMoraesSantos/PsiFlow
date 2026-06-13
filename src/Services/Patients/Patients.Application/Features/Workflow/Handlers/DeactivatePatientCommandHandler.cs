using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Patients.Application.Contracts;

namespace Patients.Application.Features.Workflow;

public sealed class DeactivatePatientCommandHandler(IPatientInviteService service) : ICommandHandler<DeactivatePatientCommand>
{
    public Task<Result> Handle(DeactivatePatientCommand command, CancellationToken cancellationToken) =>
        service.DeactivateAsync(command.PatientId, command.Reason, command.TenantId, command.UserId, command.CorrelationId, cancellationToken);
}
