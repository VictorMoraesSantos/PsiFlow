using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Patients.Application.Contracts;

namespace Patients.Application.Features.Workflow;

public sealed class PreviewPatientInviteQueryHandler(IPatientInviteService service) : IQueryHandler<PreviewPatientInviteQuery, object>
{
    public Task<Result<object>> Handle(PreviewPatientInviteQuery query, CancellationToken cancellationToken) => service.PreviewInviteAsync(query.Token, cancellationToken);
}
