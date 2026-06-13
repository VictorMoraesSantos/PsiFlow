using System.Security.Cryptography;
using System.Text;

namespace Auth.Application.Services
{
    public sealed class EncryptionService
    {
        private readonly byte[] _key;

        public EncryptionService(Auth.Application.Settings.JwtSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.EncryptionKey))
                throw new InvalidOperationException("JwtSettings:EncryptionKey is required for AES-256-GCM.");

            _key = SHA256.HashData(Encoding.UTF8.GetBytes(settings.EncryptionKey));
        }

        public EncryptedField Encrypt(string plaintext)
        {
            var nonce = RandomNumberGenerator.GetBytes(12);
            var tag = new byte[16];
            var cipher = new byte[plaintext.Length];
            using var aes = new AesGcm(_key, tag.Length);
            aes.Encrypt(nonce, Encoding.UTF8.GetBytes(plaintext), cipher, tag);
            return new EncryptedField(Convert.ToBase64String(cipher), Convert.ToBase64String(nonce), Convert.ToBase64String(tag));
        }

        public string Decrypt(EncryptedField field)
        {
            var cipher = Convert.FromBase64String(field.Ciphertext);
            var nonce = Convert.FromBase64String(field.Nonce);
            var tag = Convert.FromBase64String(field.Tag);
            var plain = new byte[cipher.Length];
            using var aes = new AesGcm(_key, tag.Length);
            aes.Decrypt(nonce, cipher, tag, plain);
            return Encoding.UTF8.GetString(plain);
        }
    }

    public sealed record EncryptedField(string Ciphertext, string Nonce, string Tag);
}
