using BuildingBlocks.Results;
using Microsoft.EntityFrameworkCore;
using Notifications.Application.Contracts;
using Notifications.Domain.Aggregates;
using PsiFlow.Notifications.Infrastructure.Persistence;

namespace Notifications.Infrastructure.Services;

public sealed class NotificationWorkflowService(NotificationsDbContext db) : INotificationWorkflowService
{
    public async Task<Result<object>> CreateTemplateVersionAsync(int templateId, string subject, string? bodyHtml, string? bodyText, int userId, CancellationToken ct)
    {
        if (!await db.NotificationTemplates.AnyAsync(x => x.Id == templateId, ct)) return Result.Failure<object>(Error.NotFound("Template not found."));
        var version = await db.NotificationTemplateVersions.CountAsync(x => x.TemplateId == templateId, ct) + 1;
        var entity = new NotificationTemplateVersion { TemplateId = templateId, Version = version, Subject = subject, BodyHtml = bodyHtml ?? string.Empty, BodyText = bodyText ?? string.Empty, CreatedBy = userId };
        db.NotificationTemplateVersions.Add(entity); await db.SaveChangesAsync(ct); return Result.Success<object>(entity);
    }
    public async Task<Result<object>> GetLogsAsync(CancellationToken ct) => Result.Success<object>(await db.NotificationLogs.AsNoTracking().OrderByDescending(x => x.Id).Take(100).ToListAsync(ct));
    public async Task<Result<object>> GetLogAsync(int notificationId, CancellationToken ct) { var log = await db.NotificationLogs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == notificationId, ct); return log is null ? Result.Failure<object>(Error.NotFound("Notification log not found.")) : Result.Success<object>(log); }
    public async Task<Result<object>> SendTestEmailAsync(string recipientEmail, string templateKey, int? tenantId, CancellationToken ct) { var log = new NotificationLog { TenantId = tenantId, RecipientEmail = recipientEmail, TemplateKey = templateKey, NotificationType = "test_email", Status = "sent", SentAt = DateTime.UtcNow }; db.NotificationLogs.Add(log); await db.SaveChangesAsync(ct); return Result.Success<object>(log); }
    public async Task<Result> RetryAsync(int notificationId, CancellationToken ct) { var log = await db.NotificationLogs.FirstOrDefaultAsync(x => x.Id == notificationId, ct); if (log is null) return Result.Failure(Error.NotFound("Notification log not found.")); log.Status = "retry_scheduled"; log.Error = null; await db.SaveChangesAsync(ct); return Result.Success(); }
    public async Task<Result<object>> ScheduleRemindersAsync(string notificationType, DateTime scheduledFor, string? recipientEmail, int? recipientUserId, int? tenantId, string? payloadJson, CancellationToken ct) { var scheduled = new ScheduledNotification { TenantId = tenantId, RecipientEmail = recipientEmail ?? string.Empty, RecipientUserId = recipientUserId, NotificationType = notificationType, ScheduledFor = scheduledFor, PayloadJson = payloadJson ?? "{}" }; db.ScheduledNotifications.Add(scheduled); await db.SaveChangesAsync(ct); return Result.Success<object>(scheduled); }
}
