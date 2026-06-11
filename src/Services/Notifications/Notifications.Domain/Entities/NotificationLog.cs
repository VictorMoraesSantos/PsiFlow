using Core.Domain.Aggregates;

namespace Notifications.Domain.Entities;

public class NotificationLog : BaseEntity<int>
{
    public int? TenantId { get; set; }
    public string RecipientEmail { get; set; } = string.Empty;
    public string Channel { get; set; } = "email";
    public string NotificationType { get; set; } = string.Empty;
    public string TemplateKey { get; set; } = string.Empty;
    public int TemplateVersion { get; set; }
    public string Provider { get; set; } = "fake";
    public string? ProviderMessageId { get; set; }
    public string Status { get; set; } = "sent";
    public string? Error { get; set; }
    public DateTime? SentAt { get; set; }
    public int? CorrelationId { get; set; }
}
