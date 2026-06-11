using BuildingBlocks.CQRS.Requests.Commands;

namespace Notifications.Application.Features.Workflow;

public sealed record ScheduleReminderCommand(string NotificationType, DateTime ScheduledFor, string? RecipientEmail, int? RecipientUserId, int? TenantId, string? PayloadJson) : ICommand<object>;
