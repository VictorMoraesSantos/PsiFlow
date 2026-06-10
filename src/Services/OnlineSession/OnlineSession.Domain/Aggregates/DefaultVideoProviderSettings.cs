using Core.Domain.Aggregates;

namespace OnlineSession.Domain.Aggregates;

public class DefaultVideoProviderSettings : BaseEntity<int>
{
    public int TenantId { get; set; }
    public string DefaultProvider { get; set; } = "external";
    public string? DefaultUrlEncrypted { get; set; }
    public string? DefaultUrlHash { get; set; }
}
