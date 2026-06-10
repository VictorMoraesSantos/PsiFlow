using BuildingBlocks.Results;

namespace Notifications.Application.Contracts;

public interface INotificationWorkflowService
{
    Task<Result<object>> CreateTemplateVersionAsync(int templateId, string subject, string? bodyHtml, string? bodyText, int userId, CancellationToken ct);
    Task<Result<object>> GetLogsAsync(CancellationToken ct);
    Task<Result<object>> GetLogAsync(int notificationId, CancellationToken ct);
    Task<Result<object>> SendTestEmailAsync(string recipientEmail, string templateKey, int? tenantId, CancellationToken ct);
    Task<Result> RetryAsync(int notificationId, CancellationToken ct);
    Task<Result<object>> ScheduleRemindersAsync(string notificationType, DateTime scheduledFor, string? recipientEmail, int? recipientUserId, int? tenantId, string? payloadJson, CancellationToken ct);
}
