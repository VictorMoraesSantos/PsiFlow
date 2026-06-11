using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using ClinicalRecords.Application.Contracts;

namespace ClinicalRecords.Application.Features.Workflow;

public sealed class AutosaveAnamnesisCommandHandler(IClinicalRecordWorkflowService service) : ICommandHandler<AutosaveAnamnesisCommand>
{
    public Task<Result> Handle(AutosaveAnamnesisCommand command, CancellationToken cancellationToken) => service.AutosaveAnamnesisAsync(command.RecordId, command.TenantId, command.Ciphertext, command.Nonce, command.Tag, cancellationToken);
}
