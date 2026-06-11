using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using OnlineSession.Application.Contracts;

namespace OnlineSession.Application.Features.Workflow;

public sealed class GetDefaultVideoSettingsQueryHandler(IOnlineSessionService service) : IQueryHandler<GetDefaultVideoSettingsQuery, object>
{
    public Task<Result<object>> Handle(GetDefaultVideoSettingsQuery query, CancellationToken cancellationToken) => service.GetDefaultSettingsAsync(query.TenantId, cancellationToken);
}
