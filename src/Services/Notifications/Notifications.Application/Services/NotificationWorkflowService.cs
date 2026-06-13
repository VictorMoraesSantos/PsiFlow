using BuildingBlocks.Results;
using Notifications.Application.Contracts;
using Notifications.Domain.Entities;
using Notifications.Domain.Repositories;

namespace Notifications.Application.Services;

public sealed class NotificationWorkflowService(
    INotificationTemplateRepository templateRepository,
    INotificationTemplateVersionRepository versionRepository,
    INotificationLogRepository logRepository,
    IScheduledNotificationRepository scheduledRepository) : INotificationWorkflowService
{
    public async Task<Result<object>> CreateTemplateVersionAsync(int templateId, string subject, string? bodyHtml, string? bodyText, int userId, CancellationToken ct)
    {
        if (!await templateRepository.AnyByIdAsync(templateId, ct)) return Result.Failure<object>(Error.NotFound("Template not found."));
        var version = await versionRepository.CountByTemplateAsync(templateId, ct) + 1;
        var entity = new NotificationTemplateVersion { TemplateId = templateId, Version = version, Subject = subject, BodyHtml = bodyHtml ?? string.Empty, BodyText = bodyText ?? string.Empty, CreatedBy = userId };
        await versionRepository.Create(entity, ct);
        return Result.Success<object>(entity);
    }

    public async Task<Result<object>> GetLogsAsync(CancellationToken ct) => Result.Success<object>(await logRepository.ListLatestAsync(100, ct));

    public async Task<Result<object>> GetLogAsync(int notificationId, CancellationToken ct)
    {
        var log = await logRepository.GetByIdAsync(notificationId, ct);
        return log is null ? Result.Failure<object>(Error.NotFound("Notification log not found.")) : Result.Success<object>(log);
    }

    public async Task<Result<object>> SendTestEmailAsync(string recipientEmail, string templateKey, int? tenantId, CancellationToken ct)
    {
        var log = new NotificationLog { TenantId = tenantId, RecipientEmail = recipientEmail, TemplateKey = templateKey, NotificationType = "test_email", Status = "sent", SentAt = DateTime.UtcNow };
        await logRepository.Create(log, ct);
        return Result.Success<object>(log);
    }

    public async Task<Result> RetryAsync(int notificationId, CancellationToken ct)
    {
        var log = await logRepository.GetById(notificationId, ct);
        if (log is null) return Result.Failure(Error.NotFound("Notification log not found."));
        log.Status = "retry_scheduled";
        log.Error = null;
        await logRepository.Update(log, ct);
        return Result.Success();
    }

    public async Task<Result<object>> ScheduleRemindersAsync(string notificationType, DateTime scheduledFor, string? recipientEmail, int? recipientUserId, int? tenantId, string? payloadJson, CancellationToken ct)
    {
        var scheduled = new ScheduledNotification { TenantId = tenantId, RecipientEmail = recipientEmail ?? string.Empty, RecipientUserId = recipientUserId, NotificationType = notificationType, ScheduledFor = scheduledFor, PayloadJson = payloadJson ?? "{}" };
        await scheduledRepository.Create(scheduled, ct);
        return Result.Success<object>(scheduled);
    }
}
