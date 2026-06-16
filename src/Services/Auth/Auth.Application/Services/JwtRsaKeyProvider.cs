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

                privateKeyPem = LoadOrCreateDevelopmentKey(environment);
                _rsa.ImportFromPem(privateKeyPem);
            }
            else
            {
                _rsa.ImportFromPem(privateKeyPem);
                if (_rsa.KeySize < 2048)
                    throw new InvalidOperationException("JWT RSA key must be at least 2048 bits.");
            }

            SigningKey = new RsaSecurityKey(_rsa) { KeyId = settings.KeyId };

            var publicKeys = new List<JsonWebKey> { BuildJwk(SigningKey) };
            foreach (var previous in settings.PreviousKeys ?? new List<JwtPreviousKey>())
            {
                if (string.IsNullOrWhiteSpace(previous.KeyId) || string.IsNullOrWhiteSpace(previous.PublicKeyPem))
                    continue;
                try
                {
                    using var rsa = RSA.Create();
                    rsa.ImportFromPem(previous.PublicKeyPem);
                    var previousKey = new RsaSecurityKey(rsa) { KeyId = previous.KeyId };
                    publicKeys.Add(BuildJwk(previousKey));
                }
                catch
                {
                    // ignora chave mal-formada para nao impedir startup
                }
            }
            PublicKeys = publicKeys;
        }

        public RsaSecurityKey SigningKey { get; }
        public IReadOnlyList<JsonWebKey> PublicKeys { get; }

        private static JsonWebKey BuildJwk(RsaSecurityKey key)
        {
            var parameters = key.Rsa is not null
                ? key.Rsa.ExportParameters(false)
                : new RSAParameters { Modulus = key.Parameters.Modulus, Exponent = key.Parameters.Exponent };

            var jwk = new JsonWebKey
            {
                Kty = "RSA",
                Kid = key.KeyId,
                Use = "sig",
                Alg = SecurityAlgorithms.RsaSha256,
                N = Base64UrlEncoder.Encode(parameters.Modulus),
                E = Base64UrlEncoder.Encode(parameters.Exponent)
            };
            return jwk;
        }

        public void Dispose() => _rsa.Dispose();

        private static string ResolvePrivateKeyPem(JwtSettings settings)
        {
            if (!string.IsNullOrWhiteSpace(settings.PrivateKeyPem)) return settings.PrivateKeyPem;
            if (string.IsNullOrWhiteSpace(settings.PrivateKeyBase64)) return string.Empty;

            return Encoding.UTF8.GetString(Convert.FromBase64String(settings.PrivateKeyBase64));
        }

        private static string LoadOrCreateDevelopmentKey(IHostEnvironment environment)
        {
            var directory = Path.Combine(environment.ContentRootPath, "Keys");
            Directory.CreateDirectory(directory);
            var keyPath = Path.Combine(directory, "auth-rsa-dev.pem");

            if (File.Exists(keyPath))
            {
                try
                {
                    var existing = File.ReadAllText(keyPath);
                    if (!string.IsNullOrWhiteSpace(existing) && existing.Contains("BEGIN", StringComparison.Ordinal))
                        return existing;
                }
                catch
                {
                    // arquivo corrompido, regerar
                }
            }

            using var rsa = RSA.Create(2048);
            var pem = rsa.ExportRSAPrivateKeyPem();
            File.WriteAllText(keyPath, pem);
            return pem;
        }
    }
}
