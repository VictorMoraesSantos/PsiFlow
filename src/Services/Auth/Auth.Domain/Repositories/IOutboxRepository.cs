using Auth.Domain.Entities;
using Auth.Domain.ValueObjects;
using Core.Domain.Repositories;

namespace Auth.Domain.Repositories
{
    public interface IOutboxRepository : IRepository<OutboxMessage, OutboxMessageId>
    {
        Task<IEnumerable<OutboxMessage>> GetUnprocessed(int maxRetries, int batchSize, CancellationToken cancellationToken = default);
    }
}
