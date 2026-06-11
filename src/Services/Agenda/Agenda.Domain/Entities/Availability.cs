using Core.Domain.Aggregates;

namespace Agenda.Domain.Entities;

public class Availability : BaseEntity<int>
{
    public int TenantId { get; set; }
    public int Weekday { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int SlotDurationMinutes { get; set; }
    public string Modality { get; set; } = "online";
    public string Timezone { get; set; } = "America/Sao_Paulo";
    public bool IsActive { get; set; } = true;
}
