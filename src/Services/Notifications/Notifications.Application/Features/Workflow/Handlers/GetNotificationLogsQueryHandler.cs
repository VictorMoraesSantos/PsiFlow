using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Notifications.Application.Contracts;

namespace Notifications.Application.Features.Workflow;

public sealed class GetNotificationLogsQueryHandler(INotificationWorkflowService service) : IQueryHandler<GetNotificationLogsQuery, object>
{
    public Task<Result<object>> Handle(GetNotificationLogsQuery query, CancellationToken cancellationToken) => service.GetLogsAsync(cancellationToken);
}
