using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using ClinicalRecords.Application.Contracts;

namespace ClinicalRecords.Application.Features.Workflow;

public sealed class PublishAnamnesisVersionCommandHandler(IClinicalRecordWorkflowService service) : ICommandHandler<PublishAnamnesisVersionCommand, object>
{
    public Task<Result<object>> Handle(PublishAnamnesisVersionCommand command, CancellationToken cancellationToken) => service.PublishAnamnesisVersionAsync(command.RecordId, command.TenantId, command.UserId, command.Reason, cancellationToken);
}
