using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Patients.Application.Contracts;

namespace Patients.Application.Features.Workflow;

public sealed class GetPatientSessionsSummaryQueryHandler(IPatientInviteService service) : IQueryHandler<GetPatientSessionsSummaryQuery, object>
{
    public Task<Result<object>> Handle(GetPatientSessionsSummaryQuery query, CancellationToken cancellationToken) => service.GetSessionsSummaryAsync(query.PatientId, query.TenantId, cancellationToken);
}
