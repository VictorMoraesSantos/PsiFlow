using Core.Domain.Filters;

namespace Patients.Domain.Filters
{
    public class PatientQueryFilter : DomainQuery
    {
        public int? TenantId { get; }
        public string? Search { get; }

        public PatientQueryFilter(int? tenantId = null, string? search = null)
        {
            TenantId = tenantId;
            Search = search;
        }
    }
}
