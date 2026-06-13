using Microsoft.EntityFrameworkCore;
using PsiFlow.Sessions.Infrastructure.Persistence.Data;
using Sessions.Domain.Entities;
using Sessions.Domain.Repositories;
using System.Linq.Expressions;

namespace Sessions.Infrastructure.Persistence.Repositories;

public sealed class ReceiptRepository(SessionsDbContext dbContext) : IReceiptRepository
{
    public Task<Receipt?> GetById(int id, CancellationToken cancellationToken = default) =>
        dbContext.Receipts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IEnumerable<Receipt?>> GetAll(CancellationToken cancellationToken = default) =>
        await dbContext.Receipts.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IEnumerable<Receipt?>> Find(Expression<Func<Receipt, bool>> predicate, CancellationToken cancellationToken = default) =>
        await dbContext.Receipts.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public async Task Create(Receipt entity, CancellationToken cancellationToken = default) =>
        await dbContext.Receipts.AddAsync(entity, cancellationToken);

    public async Task CreateRange(IEnumerable<Receipt> entities, CancellationToken cancellationToken = default) =>
        await dbContext.Receipts.AddRangeAsync(entities, cancellationToken);

    public async Task Update(Receipt entity, CancellationToken cancellationToken = default) =>
        dbContext.Receipts.Update(entity);

    public async Task Delete(Receipt entity, CancellationToken cancellationToken = default) =>
        dbContext.Receipts.Remove(entity);

    public Task<Receipt?> GetBySessionPaymentAsync(int sessionId, int paymentId, int tenantId, CancellationToken cancellationToken) =>
        dbContext.Receipts.FirstOrDefaultAsync(x => x.SessionId == sessionId && x.PaymentId == paymentId && x.TenantId == tenantId, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
