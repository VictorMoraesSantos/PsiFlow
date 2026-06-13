using Core.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using PsiFlow.Sessions.Infrastructure.Persistence.Data;
using Sessions.Domain.Entities;
using Sessions.Domain.Repositories;
using System.Linq.Expressions;

namespace Sessions.Infrastructure.Persistence.Repositories;

public sealed class ManualPaymentRepository(SessionsDbContext dbContext) : IManualPaymentRepository
{
    public Task<ManualPayment?> GetById(int id, CancellationToken cancellationToken = default) =>
        dbContext.ManualPayments.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IEnumerable<ManualPayment?>> GetAll(CancellationToken cancellationToken = default) =>
        await dbContext.ManualPayments.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IEnumerable<ManualPayment?>> Find(Expression<Func<ManualPayment, bool>> predicate, CancellationToken cancellationToken = default) =>
        await dbContext.ManualPayments.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public async Task Create(ManualPayment entity, CancellationToken cancellationToken = default) =>
        await dbContext.ManualPayments.AddAsync(entity, cancellationToken);

    public async Task CreateRange(IEnumerable<ManualPayment> entities, CancellationToken cancellationToken = default) =>
        await dbContext.ManualPayments.AddRangeAsync(entities, cancellationToken);

    public async Task Update(ManualPayment entity, CancellationToken cancellationToken = default) =>
        dbContext.ManualPayments.Update(entity);

    public async Task Delete(ManualPayment entity, CancellationToken cancellationToken = default) =>
        dbContext.ManualPayments.Remove(entity);

    public Task<ManualPayment?> GetBySessionAndTenantAsync(int sessionId, int tenantId, CancellationToken cancellationToken) =>
        dbContext.ManualPayments.FirstOrDefaultAsync(x => x.SessionId == sessionId && x.TenantId == tenantId, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
