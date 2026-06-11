using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Patients.Application.Contracts;

namespace Patients.Application.Features.Workflow;

public sealed class RevokePatientInviteCommandHandler(IPatientInviteService service) : ICommandHandler<RevokePatientInviteCommand>
{
    public Task<Result> Handle(RevokePatientInviteCommand command, CancellationToken cancellationToken) => service.RevokeInviteAsync(command.InviteId, command.TenantId, cancellationToken);
}
