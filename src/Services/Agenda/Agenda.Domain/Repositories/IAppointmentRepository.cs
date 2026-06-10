using Agenda.Domain.Aggregates;
using Agenda.Domain.Filters;
using Core.Domain.Repositories;

namespace Agenda.Domain.Repositories
{
    public interface IAppointmentRepository : IRepository<Appointment, int, AppointmentQueryFilter> { }
}
