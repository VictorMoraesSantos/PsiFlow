using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using ClinicalRecords.Application.Contracts;

namespace ClinicalRecords.Application.Features.Workflow;

public sealed class GetEvolutionVersionsQueryHandler(IClinicalRecordWorkflowService service) : IQueryHandler<GetEvolutionVersionsQuery, object>
{
    public Task<Result<object>> Handle(GetEvolutionVersionsQuery query, CancellationToken cancellationToken) => service.GetEvolutionVersionsAsync(query.SessionId, query.TenantId, cancellationToken);
}
