using Core.Domain.Aggregates;

namespace Notifications.Domain.Aggregates;

public class NotificationTemplate : BaseEntity<int>, IAggregateRoot
{
    public int? TenantId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Channel { get; set; } = "email";
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = "active";
    public bool IsActive { get; set; } = true;
}
