using Core.Domain.Aggregates;

namespace Agenda.Domain.Entities;

public class Appointment : BaseEntity<int>, IAggregateRoot
{
    public int TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int PatientId { get; set; }
    public int PsychologistId { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public string Modality { get; set; } = "online";
    public string Status { get; set; } = "scheduled";
    public bool LateCancel { get; set; }
    public DateTime? CanceledAt { get; set; }
    public int? CanceledBy { get; set; }
    public string? CancelReason { get; set; }
    public int CreatedBy { get; set; }
}
