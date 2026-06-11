using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Notifications.Application.Contracts;

namespace Notifications.Application.Features.Workflow;

public sealed class ScheduleReminderCommandHandler(INotificationWorkflowService service) : ICommandHandler<ScheduleReminderCommand, object>
{
    public Task<Result<object>> Handle(ScheduleReminderCommand command, CancellationToken cancellationToken) =>
        service.ScheduleRemindersAsync(command.NotificationType, command.ScheduledFor, command.RecipientEmail, command.RecipientUserId, command.TenantId, command.PayloadJson, cancellationToken);
}
