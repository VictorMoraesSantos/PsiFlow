using BuildingBlocks.CQRS.Requests.Queries;

namespace OnlineSession.Application.Features.Workflow;

public sealed record GetDefaultVideoSettingsQuery(int TenantId) : IQuery<object>;
