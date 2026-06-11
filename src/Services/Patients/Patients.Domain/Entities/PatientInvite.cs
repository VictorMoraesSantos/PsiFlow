using Core.Domain.Aggregates;

namespace Patients.Domain.Entities;

public class PatientInvite : BaseEntity<int>
{
    public int TenantId { get; set; }
    public int? PatientId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public string Status { get; set; } = "pending";
    public DateTime ExpiresAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public int CreatedBy { get; set; }
}
