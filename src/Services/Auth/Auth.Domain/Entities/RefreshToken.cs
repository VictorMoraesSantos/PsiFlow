using Auth.Domain.ValueObjects;
using Core.Domain.Aggregates;

namespace Auth.Domain.Entities
{
    public class RefreshToken : BaseEntity<RefreshTokenId>
    {
        public UserId UserId { get; private set; }
        public TenantId TenantId { get; private set; }
        public string TokenHash { get; private set; } = string.Empty;
        public DateTime ExpiresAt { get; private set; }
        public string? CreatedByIp { get; private set; }
        public string? UserAgent { get; private set; }
        public DateTime? RevokedAt { get; private set; }
        public string? RevokedByIp { get; private set; }
        public RefreshTokenId? ReplacedByTokenId { get; private set; }
        public new DateTime CreatedAt { get; private set; }

        protected RefreshToken() { }

        public RefreshToken(
            UserId userId,
            TenantId tenantId,
            string tokenHash,
            DateTime createdAt,
            DateTime expiresAt,
            string? createdByIp,
            string? userAgent)
        {
            UserId = userId;
            TenantId = tenantId;
            TokenHash = tokenHash;
            base.CreatedAt = createdAt;
            ExpiresAt = expiresAt;
            CreatedByIp = createdByIp;
            UserAgent = userAgent;
        }

        public bool IsActive(DateTime now) => RevokedAt is null && ExpiresAt > now;

        public void Revoke(DateTime now, string? revokedByIp, RefreshTokenId? replacedByTokenId)
        {
            RevokedAt = now;
            RevokedByIp = revokedByIp;
            ReplacedByTokenId = replacedByTokenId;
        }
    }
}
