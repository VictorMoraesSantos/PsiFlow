using Core.Domain.Aggregates;

namespace Auth.Domain.Entities
{
    public class OutboxMessage : BaseEntity<Guid>
    {
        public OutboxMessage()
        {
            Id = Guid.NewGuid();
        }

        public int AggregateId { get; set; }
        public string AggregateType { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public string Payload { get; set; } = "{}";
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; set; }
        public int RetryCount { get; set; }
        public string? Error { get; set; }
        public Guid? CorrelationId { get; set; }
    }
}
