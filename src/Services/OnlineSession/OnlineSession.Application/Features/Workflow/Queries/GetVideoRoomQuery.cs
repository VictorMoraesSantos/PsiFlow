using BuildingBlocks.CQRS.Requests.Queries;

namespace OnlineSession.Application.Features.Workflow;

public sealed record GetVideoRoomQuery(int SessionId, int TenantId) : IQuery<object>;
