using Core.Domain.Filters;

namespace ClinicalRecords.Domain.Filters;

public class MedicalRecordQueryFilter : DomainQuery
{
    public int? TenantId { get; }
    public string? Search { get; }

    public MedicalRecordQueryFilter(int? tenantId = null, string? search = null)
    {
        TenantId = tenantId;
        Search = search;
    }
}
