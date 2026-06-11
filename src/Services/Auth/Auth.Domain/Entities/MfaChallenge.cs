using Core.Domain.Aggregates;

namespace Auth.Domain.Entities
{
    public class MfaChallenge : BaseEntity<int>
    {
        public MfaChallenge() { }

        public int UserId { get; set; }
        public int TenantId { get; set; }
        public string SecretEncrypted { get; set; } = string.Empty;
        public string? QrCodeUri { get; set; }
        public bool IsConfirmed { get; set; }
        public DateTime? ConfirmedAt { get; set; }
    }
}
