using Microsoft.EntityFrameworkCore;
using OnlineSession.Domain.Entities;
using OnlineSession.Domain.Repositories;
using PsiFlow.OnlineSession.Infrastructure.Persistence.Data;
using System.Linq.Expressions;

namespace OnlineSession.Infrastructure.Persistence.Repositories;

public sealed class DefaultVideoProviderSettingsRepository(OnlineSessionDbContext dbContext) : IDefaultVideoProviderSettingsRepository
{
    public async Task<DefaultVideoProviderSettings?> GetById(int id, CancellationToken cancellationToken = default) =>
        await dbContext.DefaultVideoProviderSettings.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IEnumerable<DefaultVideoProviderSettings?>> GetAll(CancellationToken cancellationToken = default) =>
        await dbContext.DefaultVideoProviderSettings.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IEnumerable<DefaultVideoProviderSettings?>> Find(Expression<Func<DefaultVideoProviderSettings, bool>> predicate, CancellationToken cancellationToken = default) =>
        await dbContext.DefaultVideoProviderSettings.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public async Task Create(DefaultVideoProviderSettings entity, CancellationToken cancellationToken = default) =>
        await dbContext.DefaultVideoProviderSettings.AddAsync(entity, cancellationToken);

    public async Task CreateRange(IEnumerable<DefaultVideoProviderSettings> entities, CancellationToken cancellationToken = default) =>
        await dbContext.DefaultVideoProviderSettings.AddRangeAsync(entities, cancellationToken);

    public async Task Update(DefaultVideoProviderSettings entity, CancellationToken cancellationToken = default) =>
        dbContext.DefaultVideoProviderSettings.Update(entity);

    public async Task Delete(DefaultVideoProviderSettings entity, CancellationToken cancellationToken = default) =>
        dbContext.DefaultVideoProviderSettings.Remove(entity);

    public async Task<DefaultVideoProviderSettings?> GetByTenantAsync(int tenantId, CancellationToken cancellationToken = default) =>
        await dbContext.DefaultVideoProviderSettings.AsNoTracking().FirstOrDefaultAsync(x => x.TenantId == tenantId, cancellationToken);
}
