using Microsoft.EntityFrameworkCore;
using Notifications.Domain.Entities;
using Notifications.Domain.Repositories;
using PsiFlow.Notifications.Infrastructure.Persistence.Data;
using System.Linq.Expressions;

namespace Notifications.Infrastructure.Persistence.Repositories;

public sealed class NotificationTemplateVersionRepository(NotificationsDbContext dbContext) : INotificationTemplateVersionRepository
{
    public async Task<NotificationTemplateVersion?> GetById(int id, CancellationToken cancellationToken = default) =>
        await dbContext.NotificationTemplateVersions.FindAsync(new object?[] { id }, cancellationToken);

    public async Task<IEnumerable<NotificationTemplateVersion?>> GetAll(CancellationToken cancellationToken = default) =>
        await dbContext.NotificationTemplateVersions.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IEnumerable<NotificationTemplateVersion?>> Find(Expression<Func<NotificationTemplateVersion, bool>> predicate, CancellationToken cancellationToken = default) =>
        await dbContext.NotificationTemplateVersions.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public async Task Create(NotificationTemplateVersion entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await dbContext.NotificationTemplateVersions.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task CreateRange(IEnumerable<NotificationTemplateVersion> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        await dbContext.NotificationTemplateVersions.AddRangeAsync(entities, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(NotificationTemplateVersion entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = dbContext.Entry(entity);
        if (entry.State == EntityState.Detached)
            dbContext.NotificationTemplateVersions.Attach(entity);
        entry.State = EntityState.Modified;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Delete(NotificationTemplateVersion entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = dbContext.Entry(entity);
        if (entry.State == EntityState.Detached)
            dbContext.NotificationTemplateVersions.Attach(entity);
        entry.State = EntityState.Deleted;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<int> CountByTemplateAsync(int templateId, CancellationToken cancellationToken = default) =>
        dbContext.NotificationTemplateVersions.AsNoTracking().CountAsync(x => x.TemplateId == templateId, cancellationToken);
}
