using BuildingBlocks.CQRS.Requests.Queries;

namespace Notifications.Application.Features.Workflow;

public sealed record GetNotificationLogsQuery : IQuery<object>;
