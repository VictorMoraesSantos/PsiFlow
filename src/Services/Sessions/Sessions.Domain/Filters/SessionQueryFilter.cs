using Core.Domain.Filters;

namespace Sessions.Domain.Filters;

public class SessionQueryFilter : DomainQuery
{
    public int? TenantId { get; }
    public string? Search { get; }

    public SessionQueryFilter(int? tenantId = null, string? search = null)
    {
        TenantId = tenantId;
        Search = search;
    }
}
