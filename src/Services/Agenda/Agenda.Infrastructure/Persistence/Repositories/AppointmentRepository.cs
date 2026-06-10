using Agenda.Domain.Aggregates;
using Agenda.Domain.Filters;
using Agenda.Domain.Repositories;
using Core.Domain.Filters;
using Core.Infrastructure.Repositories;
using PsiFlow.Agenda.Infrastructure.Persistence;

namespace Agenda.Infrastructure.Persistence.Repositories;

public sealed class AppointmentRepository(AgendaDbContext dbContext) : Repository<Appointment, int, AppointmentQueryFilter>(dbContext), IAppointmentRepository
{
    protected override Specification<Appointment, int> CreateSpecification(AppointmentQueryFilter filter) => new AppointmentSpecification(filter);

    private sealed class AppointmentSpecification : Specification<Appointment, int>
    {
        public AppointmentSpecification(AppointmentQueryFilter filter)
        {
            ApplyBaseFilters(filter);
            AddIf(filter.TenantId.HasValue, x => x.TenantId == filter.TenantId!.Value);
            AddIf(!string.IsNullOrWhiteSpace(filter.Search), x => x.Name.Contains(filter.Search!) || x.Status.Contains(filter.Search!));
        }
    }
}
