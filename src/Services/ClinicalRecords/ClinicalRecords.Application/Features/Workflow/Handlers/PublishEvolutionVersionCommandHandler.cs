using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using ClinicalRecords.Application.Contracts;

namespace ClinicalRecords.Application.Features.Workflow;

public sealed class PublishEvolutionVersionCommandHandler(IClinicalRecordWorkflowService service) : ICommandHandler<PublishEvolutionVersionCommand, object>
{
    public Task<Result<object>> Handle(PublishEvolutionVersionCommand command, CancellationToken cancellationToken) => service.PublishEvolutionVersionAsync(command.SessionId, command.TenantId, command.UserId, command.Reason, cancellationToken);
}
