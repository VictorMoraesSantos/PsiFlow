using Microsoft.EntityFrameworkCore;
using Notifications.Domain.Entities;
using Notifications.Domain.Repositories;
using PsiFlow.Notifications.Infrastructure.Persistence.Data;
using System.Linq.Expressions;

namespace Notifications.Infrastructure.Persistence.Repositories;

public sealed class ScheduledNotificationRepository(NotificationsDbContext dbContext) : IScheduledNotificationRepository
{
    public async Task<ScheduledNotification?> GetById(int id, CancellationToken cancellationToken = default) =>
        await dbContext.ScheduledNotifications.FindAsync(new object?[] { id }, cancellationToken);

    public async Task<IEnumerable<ScheduledNotification?>> GetAll(CancellationToken cancellationToken = default) =>
        await dbContext.ScheduledNotifications.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IEnumerable<ScheduledNotification?>> Find(Expression<Func<ScheduledNotification, bool>> predicate, CancellationToken cancellationToken = default) =>
        await dbContext.ScheduledNotifications.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public async Task Create(ScheduledNotification entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await dbContext.ScheduledNotifications.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task CreateRange(IEnumerable<ScheduledNotification> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        await dbContext.ScheduledNotifications.AddRangeAsync(entities, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(ScheduledNotification entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = dbContext.Entry(entity);
        if (entry.State == EntityState.Detached)
            dbContext.ScheduledNotifications.Attach(entity);
        entry.State = EntityState.Modified;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Delete(ScheduledNotification entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = dbContext.Entry(entity);
        if (entry.State == EntityState.Detached)
            dbContext.ScheduledNotifications.Attach(entity);
        entry.State = EntityState.Deleted;
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
