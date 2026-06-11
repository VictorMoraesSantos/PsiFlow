using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using ClinicalRecords.Application.Contracts;

namespace ClinicalRecords.Application.Features.Workflow;

public sealed class GetClinicalRecordQueryHandler(IClinicalRecordWorkflowService service) : IQueryHandler<GetClinicalRecordQuery, object>
{
    public Task<Result<object>> Handle(GetClinicalRecordQuery query, CancellationToken cancellationToken) => service.GetRecordAsync(query.RecordId, query.TenantId, cancellationToken);
}
