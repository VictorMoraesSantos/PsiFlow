using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Notifications.Application.Contracts;

namespace Notifications.Application.Features.Workflow;

public sealed class RetryNotificationCommandHandler(INotificationWorkflowService service) : ICommandHandler<RetryNotificationCommand>
{
    public Task<Result> Handle(RetryNotificationCommand command, CancellationToken cancellationToken) => service.RetryAsync(command.NotificationId, cancellationToken);
}
