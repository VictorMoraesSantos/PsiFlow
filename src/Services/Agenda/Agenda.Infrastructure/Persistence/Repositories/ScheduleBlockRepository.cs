using Agenda.Domain.Entities;
using Agenda.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using PsiFlow.Agenda.Infrastructure.Persistence.Data;
using System.Linq.Expressions;

namespace Agenda.Infrastructure.Persistence.Repositories;

public sealed class ScheduleBlockRepository(AgendaDbContext dbContext) : IScheduleBlockRepository
{
    public async Task<ScheduleBlock?> GetById(int id, CancellationToken cancellationToken = default) =>
        await dbContext.ScheduleBlocks.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

    public async Task<IEnumerable<ScheduleBlock?>> GetAll(CancellationToken cancellationToken = default) =>
        await dbContext.ScheduleBlocks.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IEnumerable<ScheduleBlock?>> Find(Expression<Func<ScheduleBlock, bool>> predicate, CancellationToken cancellationToken = default) =>
        await dbContext.ScheduleBlocks.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public async Task Create(ScheduleBlock entity, CancellationToken cancellationToken = default)
    {
        await dbContext.ScheduleBlocks.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task CreateRange(IEnumerable<ScheduleBlock> entities, CancellationToken cancellationToken = default)
    {
        await dbContext.ScheduleBlocks.AddRangeAsync(entities, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(ScheduleBlock entity, CancellationToken cancellationToken = default)
    {
        var entry = dbContext.Entry(entity);
        if (entry.State == EntityState.Detached) dbContext.ScheduleBlocks.Attach(entity);
        entry.State = EntityState.Modified;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Delete(ScheduleBlock entity, CancellationToken cancellationToken = default)
    {
        var entry = dbContext.Entry(entity);
        if (entry.State == EntityState.Detached) dbContext.ScheduleBlocks.Attach(entity);
        entry.State = EntityState.Deleted;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsForPeriodAsync(int tenantId, DateTime startsAt, DateTime endsAt, CancellationToken cancellationToken = default) =>
        await dbContext.ScheduleBlocks.AnyAsync(item =>
            item.TenantId == tenantId &&
            item.StartsAt == startsAt &&
            item.EndsAt == endsAt,
            cancellationToken);

    public async Task<IReadOnlyCollection<ScheduleBlock>> ListForPeriodAsync(int tenantId, DateTime from, DateTime to, CancellationToken cancellationToken = default) =>
        await dbContext.ScheduleBlocks
            .AsNoTracking()
            .Where(item => item.TenantId == tenantId && item.StartsAt < to && item.EndsAt > from)
            .ToListAsync(cancellationToken);
}
