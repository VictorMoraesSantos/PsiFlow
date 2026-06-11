using Core.Domain.Aggregates;

namespace Patients.Domain.Entities;

public class Patient : BaseEntity<int>, IAggregateRoot
{
    public int TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateOnly? BirthDate { get; set; }
    public string? CpfEncrypted { get; set; }
    public string Status { get; set; } = "active";
    public string TreatmentStatus { get; set; } = "screening";
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public DateTime? DeactivatedAt { get; set; }
    public string? DeactivationReason { get; set; }
}
