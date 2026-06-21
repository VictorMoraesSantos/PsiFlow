namespace Auth.Application.Settings
{
    public class JwtSettings
    {
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = "psiflow-auth";
        public string Audience { get; set; } = "psiflow-api";
        public int ExpiryMinutes { get; set; } = 15;
        public int RefreshTokenExpiryDays { get; set; } = 7;
        public string EncryptionKey { get; set; } = string.Empty;
    }
}
