using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Patients.Application.Contracts;

namespace Patients.Application.Features.Workflow;

public sealed class AcceptPatientInviteCommandHandler(IPatientInviteService service) : ICommandHandler<AcceptPatientInviteCommand, object>
{
    public Task<Result<object>> Handle(AcceptPatientInviteCommand command, CancellationToken cancellationToken) => service.AcceptInviteAsync(command.Token, command.UserId, cancellationToken);
}
