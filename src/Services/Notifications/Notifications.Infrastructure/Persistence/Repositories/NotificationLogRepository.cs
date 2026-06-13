using Microsoft.EntityFrameworkCore;
using Notifications.Domain.Entities;
using Notifications.Domain.Repositories;
using PsiFlow.Notifications.Infrastructure.Persistence.Data;
using System.Linq.Expressions;

namespace Notifications.Infrastructure.Persistence.Repositories;

public sealed class NotificationLogRepository(NotificationsDbContext dbContext) : INotificationLogRepository
{
    public async Task<NotificationLog?> GetById(int id, CancellationToken cancellationToken = default) =>
        await dbContext.NotificationLogs.FindAsync(new object?[] { id }, cancellationToken);

    public async Task<NotificationLog?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await dbContext.NotificationLogs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IEnumerable<NotificationLog?>> GetAll(CancellationToken cancellationToken = default) =>
        await dbContext.NotificationLogs.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IEnumerable<NotificationLog?>> Find(Expression<Func<NotificationLog, bool>> predicate, CancellationToken cancellationToken = default) =>
        await dbContext.NotificationLogs.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public async Task Create(NotificationLog entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await dbContext.NotificationLogs.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task CreateRange(IEnumerable<NotificationLog> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        await dbContext.NotificationLogs.AddRangeAsync(entities, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(NotificationLog entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = dbContext.Entry(entity);
        if (entry.State == EntityState.Detached)
            dbContext.NotificationLogs.Attach(entity);
        entry.State = EntityState.Modified;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Delete(NotificationLog entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = dbContext.Entry(entity);
        if (entry.State == EntityState.Detached)
            dbContext.NotificationLogs.Attach(entity);
        entry.State = EntityState.Deleted;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<NotificationLog>> ListLatestAsync(int count, CancellationToken cancellationToken = default) =>
        await dbContext.NotificationLogs.AsNoTracking().OrderByDescending(x => x.Id).Take(count).ToListAsync(cancellationToken);
}
