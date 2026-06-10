using Core.Domain.Filters;
using Notifications.Domain.Aggregates;

namespace Notifications.Domain.Filters.Specifications;

public sealed class NotificationTemplateSpecification : Specification<NotificationTemplate, int>
{
    public NotificationTemplateSpecification(NotificationTemplateQueryFilter filter)
    {
        ApplyBaseFilters(filter);
        AddIf(filter.TenantId.HasValue, x => x.TenantId == filter.TenantId!.Value);
        AddIf(!string.IsNullOrWhiteSpace(filter.Search), x => x.Name.Contains(filter.Search!) || x.Key.Contains(filter.Search!));
    }
}
