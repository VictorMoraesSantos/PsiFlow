using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using OnlineSession.Application.Contracts;

namespace OnlineSession.Application.Features.Workflow;

public sealed class GetVideoRoomClicksQueryHandler(IOnlineSessionService service) : IQueryHandler<GetVideoRoomClicksQuery, object>
{
    public Task<Result<object>> Handle(GetVideoRoomClicksQuery query, CancellationToken cancellationToken) => service.GetClicksAsync(query.SessionId, query.TenantId, cancellationToken);
}
