using BuildingBlocks.CQRS.Requests.Queries;

namespace ClinicalRecords.Application.Features.Workflow;

public sealed record GetEvolutionQuery(int SessionId, int TenantId) : IQuery<object>;
