using Core.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using PsiFlow.Sessions.Infrastructure.Persistence.Data;
using Sessions.Domain.Entities;
using Sessions.Domain.Repositories;
using System.Linq.Expressions;

namespace Sessions.Infrastructure.Persistence.Repositories;

public sealed class SessionStatusHistoryRepository(SessionsDbContext dbContext) : ISessionStatusHistoryRepository
{
    public Task<SessionStatusHistory?> GetById(int id, CancellationToken cancellationToken = default) =>
        dbContext.SessionStatusHistories.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IEnumerable<SessionStatusHistory?>> GetAll(CancellationToken cancellationToken = default) =>
        await dbContext.SessionStatusHistories.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IEnumerable<SessionStatusHistory?>> Find(Expression<Func<SessionStatusHistory, bool>> predicate, CancellationToken cancellationToken = default) =>
        await dbContext.SessionStatusHistories.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public async Task Create(SessionStatusHistory entity, CancellationToken cancellationToken = default) =>
        await dbContext.SessionStatusHistories.AddAsync(entity, cancellationToken);

    public async Task CreateRange(IEnumerable<SessionStatusHistory> entities, CancellationToken cancellationToken = default) =>
        await dbContext.SessionStatusHistories.AddRangeAsync(entities, cancellationToken);

    public async Task Update(SessionStatusHistory entity, CancellationToken cancellationToken = default) =>
        dbContext.SessionStatusHistories.Update(entity);

    public async Task Delete(SessionStatusHistory entity, CancellationToken cancellationToken = default) =>
        dbContext.SessionStatusHistories.Remove(entity);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
