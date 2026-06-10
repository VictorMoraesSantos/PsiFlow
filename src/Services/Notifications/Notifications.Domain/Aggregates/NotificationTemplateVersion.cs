using Core.Domain.Aggregates;

namespace Notifications.Domain.Aggregates;

public class NotificationTemplateVersion : BaseEntity<int>
{
    public int TemplateId { get; set; }
    public int Version { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string BodyHtml { get; set; } = string.Empty;
    public string BodyText { get; set; } = string.Empty;
    public int? CreatedBy { get; set; }
    public bool IsActive { get; set; } = true;
}
