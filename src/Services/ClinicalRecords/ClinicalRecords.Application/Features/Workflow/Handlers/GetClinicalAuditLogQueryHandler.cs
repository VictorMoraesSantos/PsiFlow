using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using ClinicalRecords.Application.Contracts;

namespace ClinicalRecords.Application.Features.Workflow;

public sealed class GetClinicalAuditLogQueryHandler(IClinicalRecordWorkflowService service) : IQueryHandler<GetClinicalAuditLogQuery, object>
{
    public Task<Result<object>> Handle(GetClinicalAuditLogQuery query, CancellationToken cancellationToken) => service.GetAuditLogAsync(query.RecordId, query.TenantId, cancellationToken);
}
