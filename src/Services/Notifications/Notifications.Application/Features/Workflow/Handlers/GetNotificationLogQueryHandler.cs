using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Notifications.Application.Contracts;

namespace Notifications.Application.Features.Workflow;

public sealed class GetNotificationLogQueryHandler(INotificationWorkflowService service) : IQueryHandler<GetNotificationLogQuery, object>
{
    public Task<Result<object>> Handle(GetNotificationLogQuery query, CancellationToken cancellationToken) => service.GetLogAsync(query.NotificationId, cancellationToken);
}
