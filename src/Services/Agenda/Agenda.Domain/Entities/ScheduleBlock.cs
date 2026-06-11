using Core.Domain.Aggregates;

namespace Agenda.Domain.Entities;

public class ScheduleBlock : BaseEntity<int>
{
    public int TenantId { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public string? Reason { get; set; }
    public int CreatedBy { get; set; }
}
