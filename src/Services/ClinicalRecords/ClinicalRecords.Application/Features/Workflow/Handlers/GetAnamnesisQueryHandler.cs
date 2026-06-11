using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using ClinicalRecords.Application.Contracts;

namespace ClinicalRecords.Application.Features.Workflow;

public sealed class GetAnamnesisQueryHandler(IClinicalRecordWorkflowService service) : IQueryHandler<GetAnamnesisQuery, object>
{
    public Task<Result<object>> Handle(GetAnamnesisQuery query, CancellationToken cancellationToken) => service.GetAnamnesisAsync(query.RecordId, query.TenantId, cancellationToken);
}
