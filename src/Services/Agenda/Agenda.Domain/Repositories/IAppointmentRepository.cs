using Agenda.Domain.Entities;
using Agenda.Domain.Filters;
using Core.Domain.Repositories;

namespace Agenda.Domain.Repositories
{
    public interface IAppointmentRepository : IRepository<Appointment, int, AppointmentQueryFilter>
    {
        Task<IReadOnlyCollection<Appointment>> ListForPeriodAsync(int tenantId, DateTime from, DateTime to, string excludeStatus, CancellationToken cancellationToken = default);
        Task<Appointment?> GetByIdAndTenantAsync(int id, int tenantId, CancellationToken cancellationToken = default);
    }
}
