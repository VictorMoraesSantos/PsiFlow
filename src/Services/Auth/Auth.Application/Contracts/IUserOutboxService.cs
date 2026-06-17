using Auth.Domain.Entities;
using BuildingBlocks.Results;

namespace Auth.Application.Contracts
{
    public interface IUserOutboxService
    {
        Task<Result> PersistEventsAsync(User user, Guid correlationId, CancellationToken cancellationToken = default);
    }
}
