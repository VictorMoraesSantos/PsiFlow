using Core.Domain.Aggregates;

namespace OnlineSession.Domain.Aggregates;

public class VideoRoom : BaseEntity<int>, IAggregateRoot
{
    public int TenantId { get; set; }
    public int SessionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Provider { get; set; } = "external";
    public string UrlEncrypted { get; set; } = string.Empty;
    public string UrlHash { get; set; } = string.Empty;
    public string? Instructions { get; set; }
    public int CreatedBy { get; set; }
    public string Status { get; set; } = "active";
}
