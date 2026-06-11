using BuildingBlocks.CQRS.Requests.Queries;

namespace OnlineSession.Application.Features.Workflow;

public sealed record GetVideoRoomClicksQuery(int SessionId, int TenantId) : IQuery<object>;
