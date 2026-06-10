using Core.Domain.Aggregates;

namespace ClinicalRecords.Domain.Aggregates;

public class EvolutionVersion : BaseEntity<int>
{
    public int TenantId { get; set; }
    public int EvolutionId { get; set; }
    public int VersionNumber { get; set; }
    public string Ciphertext { get; set; } = string.Empty;
    public string Nonce { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
    public string Aad { get; set; } = string.Empty;
    public int CreatedBy { get; set; }
    public string? Reason { get; set; }
}
