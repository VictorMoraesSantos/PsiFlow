using Auth.Application.Contracts;
using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using BuildingBlocks.Results;
using Core.Domain.Events;

namespace Auth.Application.Services
{
    public class OutboxService : IOutboxService
    {
        private readonly IOutboxRepository _outboxRepository;

        public OutboxService(IOutboxRepository outboxRepository)
        {
            _outboxRepository = outboxRepository;
        }

        public async Task<Result> PersistEventsAsync(
            int aggregateId,
            string aggregateName,
            IReadOnlyCollection<IDomainEvent> events,
            Guid correlationId,
            CancellationToken cancellationToken = default)
        {
            if (events is null || events.Count == 0)
                return Result.Success();

            foreach (IDomainEvent evt in events)
            {
                var outbox = OutboxMessage.FromDomainEvent(aggregateId, aggregateName, evt, correlationId);
                await _outboxRepository.Create(outbox, cancellationToken);
            }

            return Result.Success();
        }
    }
}
