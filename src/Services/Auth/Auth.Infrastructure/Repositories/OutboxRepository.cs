using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using Auth.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Auth.Infrastructure.Repositories
{
    public class OutboxRepository(ApplicationDbContext dbContext) : IOutboxRepository
    {
        public async Task<OutboxMessage?> GetById(Guid id, CancellationToken cancellationToken = default) =>
            await dbContext.OutboxMessages.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task<IEnumerable<OutboxMessage?>> GetAll(CancellationToken cancellationToken = default) =>
            await dbContext.OutboxMessages.AsNoTracking().ToListAsync(cancellationToken);

        public async Task<IEnumerable<OutboxMessage?>> Find(Expression<Func<OutboxMessage, bool>> predicate, CancellationToken cancellationToken = default) =>
            await dbContext.OutboxMessages.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

        public async Task Create(OutboxMessage entity, CancellationToken cancellationToken = default) =>
            await dbContext.OutboxMessages.AddAsync(entity, cancellationToken);

        public async Task CreateRange(IEnumerable<OutboxMessage> entities, CancellationToken cancellationToken = default) =>
            await dbContext.OutboxMessages.AddRangeAsync(entities, cancellationToken);

        public async Task Update(OutboxMessage entity, CancellationToken cancellationToken = default) =>
            dbContext.OutboxMessages.Update(entity);

        public async Task Delete(OutboxMessage entity, CancellationToken cancellationToken = default) =>
            dbContext.OutboxMessages.Remove(entity);

        public async Task<IEnumerable<OutboxMessage>> GetUnprocessed(int maxRetries, int batchSize, CancellationToken cancellationToken = default) =>
            await dbContext.OutboxMessages
                .Where(x => x.ProcessedAt == null && x.RetryCount < maxRetries)
                .OrderBy(x => x.OccurredAt)
                .Take(batchSize)
                .ToListAsync(cancellationToken);

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
            await dbContext.SaveChangesAsync(cancellationToken);
    }
}
