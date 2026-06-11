using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using OnlineSession.Application.Contracts;

namespace OnlineSession.Application.Features.Workflow;

public sealed class GetVideoRoomQueryHandler(IOnlineSessionService service) : IQueryHandler<GetVideoRoomQuery, object>
{
    public Task<Result<object>> Handle(GetVideoRoomQuery query, CancellationToken cancellationToken) => service.GetVideoRoomAsync(query.SessionId, query.TenantId, cancellationToken);
}
