using Core.Domain.Aggregates;

namespace Notifications.Domain.Aggregates;

public class NotificationPreference : BaseEntity<int>
{
    public int TenantId { get; set; }
    public int UserId { get; set; }
    public string NotificationType { get; set; } = string.Empty;
    public bool EmailEnabled { get; set; } = true;
}
