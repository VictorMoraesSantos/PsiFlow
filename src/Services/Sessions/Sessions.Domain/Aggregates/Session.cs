using Core.Domain.Aggregates;

namespace Sessions.Domain.Aggregates;

public class Session : BaseEntity<int>, IAggregateRoot
{
    public int TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int AppointmentId { get; set; }
    public int PatientId { get; set; }
    public int PsychologistId { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public string Status { get; set; } = "scheduled";
    public string Modality { get; set; } = "online";
}
