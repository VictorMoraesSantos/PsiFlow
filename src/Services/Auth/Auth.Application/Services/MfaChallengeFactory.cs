using Auth.Application.Contracts;
using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using Auth.Domain.ValueObjects;

namespace Auth.Application.Services
{
    public class MfaChallengeFactory : IMfaChallengeFactory
    {
        private readonly IMfaChallengeRepository _mfaChallengeRepository;
        private readonly EncryptionService _encryption;

        public MfaChallengeFactory(IMfaChallengeRepository mfaChallengeRepository, EncryptionService encryption)
        {
            _mfaChallengeRepository = mfaChallengeRepository;
            _encryption = encryption;
        }

        public async Task<MfaChallenge> CreateLoginChallengeAsync(User user, CancellationToken cancellationToken = default)
        {
            var secret = MfaSecret.Generate();
            var encrypted = Encrypt(secret.Base32);
            var encryptedField = new EncryptedField(encrypted.Ciphertext, encrypted.Nonce, encrypted.Tag);
            var challenge = MfaChallenge.Start(user.Id, user.TenantId, encryptedField, qrCodeUri: null, TimeSpan.FromMinutes(10), DateTime.UtcNow);
            await _mfaChallengeRepository.Create(challenge, cancellationToken);
            return challenge;
        }

        public async Task<MfaChallenge> SetupChallengeAsync(User user, string secretBase32, string qrCodeUri, TimeSpan lifetime, CancellationToken cancellationToken = default)
        {
            var encrypted = Encrypt(secretBase32);
            var encryptedField = new EncryptedField(encrypted.Ciphertext, encrypted.Nonce, encrypted.Tag);
            var expiresAt = DateTime.UtcNow.Add(lifetime);
            var active = await _mfaChallengeRepository.GetActiveByUser(user.Id.Value, cancellationToken);
            if (active is not null)
            {
                active.SetActive(encryptedField, qrCodeUri, expiresAt);
                await _mfaChallengeRepository.Update(active, cancellationToken);
                return active;
            }

            var challenge = MfaChallenge.Start(user.Id, user.TenantId, encryptedField, qrCodeUri, lifetime, DateTime.UtcNow);
            await _mfaChallengeRepository.Create(challenge, cancellationToken);
            return challenge;
        }

        public string DecryptSecret(MfaChallenge challenge)
        {
            var encrypted = new EncryptedField(challenge.SecretCiphertext, challenge.SecretNonce, challenge.SecretTag);
            var plain = _encryption.Decrypt(encrypted);
            return plain;
        }

        private EncryptedField Encrypt(string plain)
        {
            var encrypted = _encryption.Encrypt(plain);
            var field = new EncryptedField(encrypted.Ciphertext, encrypted.Nonce, encrypted.Tag);
            return field;
        }
    }
}
