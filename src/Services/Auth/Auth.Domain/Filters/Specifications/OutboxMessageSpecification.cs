using Auth.Domain.Entities;
using Auth.Domain.ValueObjects;
using Core.Domain.Filters;

namespace Auth.Domain.Filters.Specifications
{
    public class OutboxMessageSpecification : Specification<OutboxMessage, OutboxMessageId>
    {
        public OutboxMessageSpecification(OutboxMessageFilter filter)
        {
            ApplyBaseFilters(filter);
            AddIf(filter.AggregateId.HasValue, o => o.AggregateId == filter.AggregateId!.Value);
            AddIf(!string.IsNullOrWhiteSpace(filter.AggregateType), o => o.AggregateType == filter.AggregateType);
            AddIf(!string.IsNullOrWhiteSpace(filter.EventType), o => o.EventType == filter.EventType);
            AddIf(filter.IsProcessed.HasValue, o => filter.IsProcessed!.Value ? o.ProcessedAt != null : o.ProcessedAt == null);
            AddIf(filter.CorrelationId.HasValue, o => o.CorrelationId == filter.CorrelationId!.Value);
            AddIf(filter.MinRetryCount.HasValue, o => o.RetryCount >= filter.MinRetryCount!.Value);
            AddIf(filter.MaxRetryCount.HasValue, o => o.RetryCount <= filter.MaxRetryCount!.Value);
        }
    }
}
