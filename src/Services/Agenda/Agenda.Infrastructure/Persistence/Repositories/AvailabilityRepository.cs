using Agenda.Domain.Entities;
using Agenda.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using PsiFlow.Agenda.Infrastructure.Persistence.Data;
using System.Linq.Expressions;

namespace Agenda.Infrastructure.Persistence.Repositories;

public sealed class AvailabilityRepository(AgendaDbContext dbContext) : IAvailabilityRepository
{
    public async Task<Availability?> GetById(int id, CancellationToken cancellationToken = default) =>
        await dbContext.Availabilities.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

    public async Task<IEnumerable<Availability?>> GetAll(CancellationToken cancellationToken = default) =>
        await dbContext.Availabilities.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IEnumerable<Availability?>> Find(Expression<Func<Availability, bool>> predicate, CancellationToken cancellationToken = default) =>
        await dbContext.Availabilities.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public async Task Create(Availability entity, CancellationToken cancellationToken = default)
    {
        await dbContext.Availabilities.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task CreateRange(IEnumerable<Availability> entities, CancellationToken cancellationToken = default)
    {
        await dbContext.Availabilities.AddRangeAsync(entities, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(Availability entity, CancellationToken cancellationToken = default)
    {
        var entry = dbContext.Entry(entity);
        if (entry.State == EntityState.Detached) dbContext.Availabilities.Attach(entity);
        entry.State = EntityState.Modified;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Delete(Availability entity, CancellationToken cancellationToken = default)
    {
        var entry = dbContext.Entry(entity);
        if (entry.State == EntityState.Detached) dbContext.Availabilities.Attach(entity);
        entry.State = EntityState.Deleted;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> GetOverlappingAsync(int tenantId, int weekday, string modality, TimeOnly startTime, TimeOnly endTime, int? excludedId, CancellationToken cancellationToken = default) =>
        await dbContext.Availabilities.AnyAsync(item =>
            item.TenantId == tenantId &&
            item.Id != excludedId &&
            item.Weekday == weekday &&
            item.Modality == modality &&
            item.StartTime < endTime &&
            item.EndTime > startTime,
            cancellationToken);

    public async Task ReplaceTenantWeekAsync(int tenantId, IEnumerable<Availability> availabilities, CancellationToken cancellationToken = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        var existing = await dbContext.Availabilities.Where(item => item.TenantId == tenantId).ToListAsync(cancellationToken);
        dbContext.Availabilities.RemoveRange(existing);
        await dbContext.Availabilities.AddRangeAsync(availabilities, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}
