using Core.Domain.Aggregates;

namespace Notifications.Domain.Aggregates;

public class ScheduledNotification : BaseEntity<int>
{
    public int? TenantId { get; set; }
    public int? RecipientUserId { get; set; }
    public string RecipientEmail { get; set; } = string.Empty;
    public string NotificationType { get; set; } = string.Empty;
    public DateTime ScheduledFor { get; set; }
    public string PayloadJson { get; set; } = "{}";
    public string Status { get; set; } = "pending";
    public int AttemptCount { get; set; }
    public string? LastError { get; set; }
}
