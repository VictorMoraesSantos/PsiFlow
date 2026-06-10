using Core.Domain.Filters;
using Sessions.Domain.Aggregates;

namespace Sessions.Domain.Filters.Specifications;

public sealed class SessionSpecification : Specification<Session, int>
{
    public SessionSpecification(SessionQueryFilter filter)
    {
        ApplyBaseFilters(filter);
        AddIf(filter.TenantId.HasValue, x => x.TenantId == filter.TenantId!.Value);
        AddIf(!string.IsNullOrWhiteSpace(filter.Search), x => x.Name.Contains(filter.Search!) || x.Status.Contains(filter.Search!));
    }
}
