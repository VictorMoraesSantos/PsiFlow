namespace Notifications.Application.Features.Workflow;

public sealed record ScheduleReminderBody(string NotificationType, DateTime ScheduledFor, string? RecipientEmail, int? RecipientUserId, string? PayloadJson);
