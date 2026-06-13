using Auth.Application.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace Auth.Application.Services
{
    public sealed class JwtRsaKeyProvider : IDisposable
    {
        private readonly RSA _rsa;

        public JwtRsaKeyProvider(JwtSettings settings, IHostEnvironment environment)
        {
            _rsa = RSA.Create();
            var privateKeyPem = ResolvePrivateKeyPem(settings);

            if (string.IsNullOrWhiteSpace(privateKeyPem))
            {
                if (!environment.IsDevelopment())
                    throw new InvalidOperationException("JwtSettings:PrivateKeyPem or JwtSettings:PrivateKeyBase64 must be configured outside Development.");

                _rsa.KeySize = 2048;
            }
            else
            {
                _rsa.ImportFromPem(privateKeyPem);
                if (_rsa.KeySize < 2048)
                    throw new InvalidOperationException("JWT RSA key must be at least 2048 bits.");
            }

            SigningKey = new RsaSecurityKey(_rsa) { KeyId = settings.KeyId };
        }

        public RsaSecurityKey SigningKey { get; }

        public JsonWebKey PublicJwk => JsonWebKeyConverter.ConvertFromRSASecurityKey(SigningKey);

        public void Dispose() => _rsa.Dispose();

        private static string ResolvePrivateKeyPem(JwtSettings settings)
        {
            if (!string.IsNullOrWhiteSpace(settings.PrivateKeyPem)) return settings.PrivateKeyPem;
            if (string.IsNullOrWhiteSpace(settings.PrivateKeyBase64)) return string.Empty;

            return Encoding.UTF8.GetString(Convert.FromBase64String(settings.PrivateKeyBase64));
        }
    }
}
