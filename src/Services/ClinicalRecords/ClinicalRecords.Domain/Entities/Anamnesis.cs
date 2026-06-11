using Core.Domain.Aggregates;

namespace ClinicalRecords.Domain.Entities;

public class Anamnesis : BaseEntity<int>
{
    public int TenantId { get; set; }
    public int RecordId { get; set; }
    public int? CurrentVersionId { get; set; }
    public string? DraftCiphertext { get; set; }
    public string? DraftNonce { get; set; }
    public string? DraftTag { get; set; }
    public DateTime? DraftUpdatedAt { get; set; }
}
