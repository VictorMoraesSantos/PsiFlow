using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using ClinicalRecords.Application.Contracts;

namespace ClinicalRecords.Application.Features.Workflow;

public sealed class GetEvolutionQueryHandler(IClinicalRecordWorkflowService service) : IQueryHandler<GetEvolutionQuery, object>
{
    public Task<Result<object>> Handle(GetEvolutionQuery query, CancellationToken cancellationToken) => service.GetEvolutionAsync(query.SessionId, query.TenantId, cancellationToken);
}
