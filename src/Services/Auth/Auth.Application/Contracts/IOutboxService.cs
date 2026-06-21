using BuildingBlocks.Results;
using Core.Domain.Events;

namespace Auth.Application.Contracts
{
    public interface IOutboxService
    {
        Task<Result> PersistEventsAsync(
            int aggregateId,
            string aggregateName,
            IReadOnlyCollection<IDomainEvent> events,
            Guid correlationId,
            CancellationToken cancellationToken = default);
    }
}
