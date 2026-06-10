using Core.Domain.Filters;

namespace Notifications.Domain.Filters;

public class NotificationTemplateQueryFilter : DomainQuery
{
    public int? TenantId { get; }
    public string? Search { get; }

    public NotificationTemplateQueryFilter(int? tenantId = null, string? search = null)
    {
        TenantId = tenantId;
        Search = search;
    }
}
