using Agenda.Domain.Entities;
using Agenda.Domain.Filters;
using Agenda.Domain.Filters.Specifications;
using Agenda.Domain.Repositories;
using Core.Domain.Filters;
using Core.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using PsiFlow.Agenda.Infrastructure.Persistence.Data;

namespace Agenda.Infrastructure.Persistence.Repositories;

public sealed class AppointmentRepository(AgendaDbContext dbContext) : Repository<Appointment, int, AppointmentQueryFilter>(dbContext), IAppointmentRepository
{
    protected override Specification<Appointment, int> CreateSpecification(AppointmentQueryFilter filter) => new AppointmentSpecification(filter);

    public async Task<IReadOnlyCollection<Appointment>> ListForPeriodAsync(int tenantId, DateTime from, DateTime to, string excludeStatus, CancellationToken cancellationToken = default) =>
        await dbContext.Appointments
            .AsNoTracking()
            .Where(item => item.TenantId == tenantId && item.StartsAt < to && item.EndsAt > from && item.Status != excludeStatus)
            .ToListAsync(cancellationToken);

    public async Task<Appointment?> GetByIdAndTenantAsync(int id, int tenantId, CancellationToken cancellationToken = default) =>
        await dbContext.Appointments.FirstOrDefaultAsync(item => item.Id == id && item.TenantId == tenantId, cancellationToken);
}
