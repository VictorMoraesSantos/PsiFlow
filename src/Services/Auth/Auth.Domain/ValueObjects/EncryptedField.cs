using Auth.Domain.Errors;
using Core.Domain.Exceptions;

namespace Auth.Domain.ValueObjects
{
    public sealed class EncryptedField
    {
        public string Ciphertext { get; }
        public string Nonce { get; }
        public string Tag { get; }

        public EncryptedField(string ciphertext, string nonce, string tag)
        {
            if (string.IsNullOrWhiteSpace(ciphertext)) throw new DomainException(MfaChallengeErrors.SecretRequired);
            if (string.IsNullOrWhiteSpace(nonce)) throw new DomainException(MfaChallengeErrors.SecretRequired);
            if (string.IsNullOrWhiteSpace(tag)) throw new DomainException(MfaChallengeErrors.SecretRequired);
            if (ciphertext.Length < 16) throw new DomainException(MfaChallengeErrors.SecretTooShort);

            Ciphertext = ciphertext;
            Nonce = nonce;
            Tag = tag;
        }
    }
}
