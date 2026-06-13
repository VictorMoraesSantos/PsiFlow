namespace Auth.Application.Settings
{
    public class JwtSettings
    {
        public string Key { get; set; } = string.Empty;
        public string PrivateKeyPem { get; set; } = string.Empty;
        public string PrivateKeyBase64 { get; set; } = string.Empty;
        public string KeyId { get; set; } = "psiflow-auth-rsa-1";
        public string Issuer { get; set; } = "psiflow-auth";
        public string Audience { get; set; } = "psiflow-api";
        public int ExpiryMinutes { get; set; } = 15;
        public int RefreshTokenExpiryDays { get; set; } = 7;
    }
}
