using Core.Domain.Filters;

namespace Agenda.Domain.Filters;

public class AppointmentQueryFilter : DomainQuery
{
    public int? TenantId { get; }
    public string? Search { get; }

    public AppointmentQueryFilter(int? tenantId = null, string? search = null)
    {
        TenantId = tenantId;
        Search = search;
    }
}
