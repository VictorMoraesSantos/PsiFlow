using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using ClinicalRecords.Application.Contracts;

namespace ClinicalRecords.Application.Features.Workflow;

public sealed class AutosaveEvolutionCommandHandler(IClinicalRecordWorkflowService service) : ICommandHandler<AutosaveEvolutionCommand>
{
    public Task<Result> Handle(AutosaveEvolutionCommand command, CancellationToken cancellationToken) => service.AutosaveEvolutionAsync(command.SessionId, command.TenantId, command.Ciphertext, command.Nonce, command.Tag, cancellationToken);
}
