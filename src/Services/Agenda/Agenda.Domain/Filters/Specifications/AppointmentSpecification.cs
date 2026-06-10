using Agenda.Domain.Aggregates;
using Core.Domain.Filters;

namespace Agenda.Domain.Filters.Specifications;

public sealed class AppointmentSpecification : Specification<Appointment, int>
{
    public AppointmentSpecification(AppointmentQueryFilter filter)
    {
        ApplyBaseFilters(filter);
        AddIf(filter.TenantId.HasValue, x => x.TenantId == filter.TenantId!.Value);
        AddIf(!string.IsNullOrWhiteSpace(filter.Search), x => x.Name.Contains(filter.Search!) || x.Status.Contains(filter.Search!));
    }
}
