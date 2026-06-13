using Agenda.Domain.Entities;
using Agenda.Domain.Filters;
using Agenda.Domain.Filters.Specifications;
using Agenda.Domain.Repositories;
using Core.Domain.Filters;
using Core.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using PsiFlow.Agenda.Infrastructure.Persistence.Data;
using System.Data;

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

    public async Task<bool> CreateIfSlotIsFreeAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        var hasConflict = await dbContext.Appointments.AnyAsync(item =>
            item.TenantId == appointment.TenantId &&
            item.PsychologistId == appointment.PsychologistId &&
            item.Status != "canceled" &&
            item.StartsAt < appointment.EndsAt &&
            item.EndsAt > appointment.StartsAt,
            cancellationToken);

        if (hasConflict) return false;

        await dbContext.Appointments.AddAsync(appointment, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return true;
    }
}
