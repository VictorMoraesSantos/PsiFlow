using Auth.Domain.Errors;
using Auth.Domain.ValueObjects;
using Core.Domain.Aggregates;
using Core.Domain.Exceptions;

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

        protected RefreshToken(
            UserId userId,
            TenantId tenantId,
            string tokenHash,
            DateTime createdAt,
            DateTime expiresAt,
            string? createdByIp,
            string? userAgent)
        {
            if (string.IsNullOrWhiteSpace(tokenHash))
                throw new DomainException(RefreshTokenErrors.InvalidId);
            if (expiresAt <= createdAt)
                throw new DomainException(RefreshTokenErrors.InvalidId);

            UserId = userId;
            TenantId = tenantId;
            TokenHash = tokenHash;
            base.CreatedAt = createdAt;
            ExpiresAt = expiresAt;
            CreatedByIp = createdByIp;
            UserAgent = userAgent;
        }

        public static RefreshToken Issue(
            UserId userId,
            TenantId tenantId,
            string tokenHash,
            DateTime createdAt,
            TimeSpan lifetime,
            string? createdByIp,
            string? userAgent)
        {
            if (lifetime <= TimeSpan.Zero)
                throw new DomainException(RefreshTokenErrors.InvalidId);
            return new RefreshToken(userId, tenantId, tokenHash, createdAt, createdAt.Add(lifetime), createdByIp, userAgent);
        }

        public bool IsActive(DateTime now) => RevokedAt is null && ExpiresAt > now;
        public bool IsRevoked() => RevokedAt is not null;
        public bool IsExpired(DateTime now) => ExpiresAt <= now;
        public bool BelongsTo(int userId) => UserId.Value == userId;

        public RefreshToken Rotate(
            string newTokenHash,
            DateTime now,
            TimeSpan lifetime)
        {
            if (IsRevoked())
                throw new DomainException(RefreshTokenErrors.Reused);

            var replacement = new RefreshToken(
                UserId,
                TenantId,
                newTokenHash,
                now,
                now.Add(lifetime),
                CreatedByIp,
                UserAgent);

            Revoke(now, CreatedByIp, replacement.Id);
            return replacement;
        }

        public void Revoke(DateTime now, string? revokedByIp, RefreshTokenId? replacedByTokenId)
        {
            if (RevokedAt is not null) return;
            RevokedAt = now;
            RevokedByIp = revokedByIp;
            ReplacedByTokenId = replacedByTokenId;
            MarkAsUpdated();
        }

        public void RevokeAsReused(DateTime now)
        {
            if (RevokedAt is not null) return;
            Revoke(now, RevokedByIp, ReplacedByTokenId);
        }
    }
}
