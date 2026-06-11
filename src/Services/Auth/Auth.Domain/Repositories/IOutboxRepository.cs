using Auth.Domain.Entities;
using Core.Domain.Repositories;

namespace Auth.Domain.Repositories
{
    public interface IOutboxRepository : IRepository<OutboxMessage, Guid>
    {
        Task<IEnumerable<OutboxMessage>> GetUnprocessedAsync(int maxRetries, int batchSize, CancellationToken cancellationToken = default);
    }
}
