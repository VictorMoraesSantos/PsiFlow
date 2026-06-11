using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Sessions.Application.Contracts;

namespace Sessions.Application.Features.Workflow;

public sealed class GetPatientSessionsQueryHandler(ISessionWorkflowService service) : IQueryHandler<GetPatientSessionsQuery, IReadOnlyCollection<SessionResult>>
{
    public Task<Result<IReadOnlyCollection<SessionResult>>> Handle(GetPatientSessionsQuery query, CancellationToken cancellationToken) =>
        service.GetPatientSessionsAsync(query.PatientId, query.TenantId, cancellationToken);
}
