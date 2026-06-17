using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using Auth.Domain.ValueObjects;
using Auth.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Auth.Infrastructure.Repositories
{
    public class OutboxRepository(ApplicationDbContext dbContext) : IOutboxRepository
    {
        public async Task<OutboxMessage?> GetById(OutboxMessageId id, CancellationToken cancellationToken = default)
        {
            var message = await dbContext.OutboxMessages.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            return message;
        }

        public async Task<IEnumerable<OutboxMessage?>> GetAll(CancellationToken cancellationToken = default)
        {
            var messages = await dbContext.OutboxMessages.AsNoTracking().ToListAsync(cancellationToken);
            return messages;
        }

        public async Task<IEnumerable<OutboxMessage?>> Find(Expression<Func<OutboxMessage, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var messages = await dbContext.OutboxMessages.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
            return messages;
        }

        public async Task Create(OutboxMessage entity, CancellationToken cancellationToken = default)
        {
            await dbContext.OutboxMessages.AddAsync(entity, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateRange(IEnumerable<OutboxMessage> entities, CancellationToken cancellationToken = default)
        {
            await dbContext.OutboxMessages.AddRangeAsync(entities, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task Update(OutboxMessage entity, CancellationToken cancellationToken = default)
        {
            dbContext.OutboxMessages.Update(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(OutboxMessage entity, CancellationToken cancellationToken = default)
        {
            dbContext.OutboxMessages.Remove(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<OutboxMessage>> GetUnprocessed(int maxRetries, int batchSize, CancellationToken cancellationToken = default)
        {
            var messages = await dbContext.OutboxMessages
                .Where(x => x.ProcessedAt == null && x.RetryCount < maxRetries)
                .OrderBy(x => x.OccurredAt)
                .Take(batchSize)
                .ToListAsync(cancellationToken);
            return messages;
        }
    }
}
