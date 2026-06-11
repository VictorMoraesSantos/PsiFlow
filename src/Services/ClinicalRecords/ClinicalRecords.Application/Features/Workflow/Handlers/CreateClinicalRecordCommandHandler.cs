using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using ClinicalRecords.Application.Contracts;

namespace ClinicalRecords.Application.Features.Workflow;

public sealed class CreateClinicalRecordCommandHandler(IClinicalRecordWorkflowService service) : ICommandHandler<CreateClinicalRecordCommand, object>
{
    public Task<Result<object>> Handle(CreateClinicalRecordCommand command, CancellationToken cancellationToken) => service.CreateRecordAsync(command.PatientId, command.TenantId, command.Name, cancellationToken);
}
