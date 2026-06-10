using Core.Domain.Filters;
using Patients.Domain.Aggregates;

namespace Patients.Domain.Filters.Specifications;

public sealed class PatientSpecification : Specification<Patient, int>
{
    public PatientSpecification(PatientQueryFilter filter)
    {
        ApplyBaseFilters(filter);
        AddIf(filter.TenantId.HasValue, x => x.TenantId == filter.TenantId!.Value);
        AddIf(!string.IsNullOrWhiteSpace(filter.Search), x => x.Name.Contains(filter.Search!) || x.FullName.Contains(filter.Search!) || x.Email.Contains(filter.Search!));
    }
}
