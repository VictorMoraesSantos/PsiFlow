using ClinicalRecords.Domain.Aggregates;
using Core.Domain.Filters;

namespace ClinicalRecords.Domain.Filters.Specifications;

public sealed class MedicalRecordSpecification : Specification<MedicalRecord, int>
{
    public MedicalRecordSpecification(MedicalRecordQueryFilter filter)
    {
        ApplyBaseFilters(filter);
        AddIf(filter.TenantId.HasValue, x => x.TenantId == filter.TenantId!.Value);
        AddIf(!string.IsNullOrWhiteSpace(filter.Search), x => x.Name.Contains(filter.Search!) || x.Status.Contains(filter.Search!));
    }
}
