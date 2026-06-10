using Agenda.Domain.Aggregates;
using Agenda.Domain.Filters;
using Agenda.Domain.Filters.Specifications;
using Agenda.Domain.Repositories;
using Core.Domain.Filters;
using Core.Infrastructure.Repositories;
using PsiFlow.Agenda.Infrastructure.Persistence;

namespace Agenda.Infrastructure.Persistence.Repositories;

public sealed class AppointmentRepository(AgendaDbContext dbContext) : Repository<Appointment, int, AppointmentQueryFilter>(dbContext), IAppointmentRepository
{
    protected override Specification<Appointment, int> CreateSpecification(AppointmentQueryFilter filter) => new AppointmentSpecification(filter);
}
