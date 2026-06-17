using Auth.Application.Contracts;
using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using BuildingBlocks.Results;
using Core.Domain.Events;

namespace Auth.Application.Services
{
    public class UserOutboxService : IUserOutboxService
    {
        private readonly IOutboxRepository _outboxRepository;
        private readonly IUserRepository _userRepository;

        public UserOutboxService(IOutboxRepository outboxRepository, IUserRepository userRepository)
        {
            _outboxRepository = outboxRepository;
            _userRepository = userRepository;
        }

        public async Task<Result> PersistEventsAsync(User user, Guid correlationId, CancellationToken cancellationToken = default)
        {
            foreach (IDomainEvent evt in user.DomainEvents)
            {
                var outbox = OutboxMessage.FromDomainEvent(user.Id.Value, nameof(User), evt, correlationId);
                await _outboxRepository.Create(outbox, cancellationToken);
            }
            user.ClearDomainEvents();
            await _userRepository.Update(user, cancellationToken);

            var success = Result.Success();
            return success;
        }
    }
}
