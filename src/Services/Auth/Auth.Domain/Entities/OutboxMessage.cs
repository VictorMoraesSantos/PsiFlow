using Auth.Domain.Errors;
using Auth.Domain.ValueObjects;
using Core.Domain.Aggregates;
using Core.Domain.Events;
using Core.Domain.Exceptions;

namespace Auth.Domain.Entities
{
    public class OutboxMessage : BaseEntity<OutboxMessageId>
    {
        public int AggregateId { get; private set; }
        public string AggregateType { get; private set; } = string.Empty;
        public string EventType { get; private set; } = string.Empty;
        public string Payload { get; private set; } = "{}";
        public DateTime OccurredAt { get; private set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; private set; }
        public int RetryCount { get; private set; }
        public string? Error { get; private set; }
        public Guid? CorrelationId { get; private set; }

        public OutboxMessage() { }

        public OutboxMessage(
            int aggregateId,
            string aggregateType,
            string eventType,
            string payload,
            DateTime occurredAt,
            DateTime? processedAt,
            int retryCount,
            string? error,
            Guid? correlationId)
        {
            if (string.IsNullOrWhiteSpace(aggregateType))
                throw new DomainException(OutboxMessageErrors.AggregateTypeRequired);
            if (string.IsNullOrWhiteSpace(eventType))
                throw new DomainException(OutboxMessageErrors.EventTypeRequired);
            if (string.IsNullOrWhiteSpace(payload))
                throw new DomainException(OutboxMessageErrors.PayloadRequired);

            AggregateId = aggregateId;
            AggregateType = aggregateType.Trim();
            EventType = eventType.Trim();
            Payload = payload;
            OccurredAt = occurredAt;
            ProcessedAt = processedAt;
            RetryCount = retryCount;
            Error = error;
            CorrelationId = correlationId;
        }

        public static OutboxMessage Enqueue(
            int aggregateId,
            string aggregateType,
            string eventType,
            string payload,
            DateTime occurredAt,
            Guid? correlationId)
        {
            return new OutboxMessage(
                aggregateId,
                aggregateType,
                eventType,
                payload,
                occurredAt,
                null,
                0,
                null,
                correlationId);
        }

        public static OutboxMessage FromDomainEvent(
            int aggregateId,
            string aggregateType,
            IDomainEvent @event,
            Guid? correlationId)
        {
            if (@event is null)
                throw new DomainException(OutboxMessageErrors.PayloadRequired);

            var payload = System.Text.Json.JsonSerializer.Serialize(@event);
            return Enqueue(aggregateId, aggregateType, @event.GetType().Name, payload, @event.OccuredOn, correlationId);
        }

        public void MarkProcessed(DateTime processedAt)
        {
            if (ProcessedAt is not null)
                throw new DomainException(OutboxMessageErrors.AlreadyProcessed);
            ProcessedAt = processedAt;
            Error = null;
        }

        public void RecordFailure(string error)
        {
            RetryCount++;
            Error = error;
        }

        public bool CanRetry(int maxRetries) => RetryCount < maxRetries;
        public bool IsPending() => ProcessedAt is null;
    }
}
