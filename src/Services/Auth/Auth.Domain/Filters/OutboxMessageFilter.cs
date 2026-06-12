using Core.Domain.Filters;

namespace Auth.Domain.Filters
{
    public class OutboxMessageFilter : DomainQuery
    {
        public int? AggregateId { get; private set; }
        public string? AggregateType { get; private set; }
        public string? EventType { get; private set; }
        public bool? IsProcessed { get; private set; }
        public Guid? CorrelationId { get; private set; }
        public int? MinRetryCount { get; private set; }
        public int? MaxRetryCount { get; private set; }

        public OutboxMessageFilter(
            int? aggregateId = null,
            string? aggregateType = null,
            string? eventType = null,
            bool? isProcessed = null,
            Guid? correlationId = null,
            int? minRetryCount = null,
            int? maxRetryCount = null)
        {
            AggregateId = aggregateId;
            AggregateType = aggregateType;
            EventType = eventType;
            IsProcessed = isProcessed;
            CorrelationId = correlationId;
            MinRetryCount = minRetryCount;
            MaxRetryCount = maxRetryCount;
        }
    }
}
