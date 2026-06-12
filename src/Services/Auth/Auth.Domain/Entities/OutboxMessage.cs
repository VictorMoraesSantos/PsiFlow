using Auth.Domain.ValueObjects;
using Core.Domain.Aggregates;

namespace Auth.Domain.Entities
{
    public class OutboxMessage : BaseEntity<OutboxMessageId>
    {
        public int AggregateId { get; set; }
        public string AggregateType { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public string Payload { get; set; } = "{}";
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; set; }
        public int RetryCount { get; set; }
        public string? Error { get; set; }
        public Guid? CorrelationId { get; set; }

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
            AggregateId = aggregateId;
            AggregateType = aggregateType;
            EventType = eventType;
            Payload = payload;
            OccurredAt = occurredAt;
            ProcessedAt = processedAt;
            RetryCount = retryCount;
            Error = error;
            CorrelationId = correlationId;
        }
    }
}
