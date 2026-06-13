using Auth.Domain.ValueObjects;
using Core.Domain.Aggregates;
using Core.Domain.Exceptions;
using System.Security.Cryptography;
using System.Text;

namespace Auth.Domain.Entities
{
    public class Consent : BaseEntity<ConsentId>
    {
        public UserId UserId { get; private set; }
        public TenantId TenantId { get; private set; }
        public string DocumentType { get; private set; } = "terms_privacy";
        public string Version { get; private set; } = string.Empty;
        public string TermsVersion { get; private set; } = string.Empty;
        public string PrivacyVersion { get; private set; } = string.Empty;
        public string DocumentHash { get; private set; } = string.Empty;
        public string? IpAddress { get; private set; }
        public string? UserAgent { get; private set; }
        public DateTime AcceptedAt { get; private set; } = DateTime.UtcNow;

        protected Consent() { }

        protected Consent(
            UserId userId,
            TenantId tenantId,
            string documentType,
            string version,
            string termsVersion,
            string privacyVersion,
            string documentHash,
            string? ipAddress,
            string? userAgent,
            DateTime acceptedAt)
        {
            UserId = userId;
            TenantId = tenantId;
            DocumentType = string.IsNullOrWhiteSpace(documentType) ? "terms_privacy" : documentType;
            Version = string.IsNullOrWhiteSpace(version) ? $"{termsVersion}/{privacyVersion}" : version;
            TermsVersion = termsVersion;
            PrivacyVersion = privacyVersion;
            DocumentHash = documentHash;
            IpAddress = ipAddress;
            UserAgent = userAgent;
            AcceptedAt = acceptedAt;
        }

        public static Consent Accept(
            UserId userId,
            TenantId tenantId,
            DocumentVersion termsVersion,
            DocumentVersion privacyVersion,
            string documentType,
            string? ipAddress,
            string? userAgent,
            DateTime acceptedAt)
        {
            var hash = ComputeHash(
                $"{userId}:{(string.IsNullOrWhiteSpace(documentType) ? "terms_privacy" : documentType)}:{termsVersion.Value}:{privacyVersion.Value}");

            return new Consent(
                userId,
                tenantId,
                documentType,
                $"{termsVersion.Value}/{privacyVersion.Value}",
                termsVersion.Value,
                privacyVersion.Value,
                hash,
                ipAddress,
                userAgent,
                acceptedAt);
        }

        public bool MatchesVersions(DocumentVersion termsVersion, DocumentVersion privacyVersion) =>
            TermsVersion == termsVersion.Value && PrivacyVersion == privacyVersion.Value;

        private static string ComputeHash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes);
        }
    }
}
