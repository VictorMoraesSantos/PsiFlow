using Auth.Domain.Errors;
using Core.Domain.Exceptions;
using System.Security.Cryptography;
using System.Text;

namespace Auth.Domain.ValueObjects
{
    public sealed class MfaSecret
    {
        public string Base32 { get; }

        private MfaSecret(string base32)
        {
            Base32 = base32;
        }

        public static MfaSecret Generate()
        {
            var bytes = RandomNumberGenerator.GetBytes(20);
            return new MfaSecret(EncodeBase32(bytes));
        }

        public static MfaSecret FromBase32(string base32)
        {
            if (string.IsNullOrWhiteSpace(base32))
                throw new DomainException(MfaChallengeErrors.SecretRequired);
            return new MfaSecret(base32.Trim());
        }

        public byte[] ToBytes() => DecodeBase32(Base32);

        public string BuildQrCodeUri(string account, string issuer = "PsiFlow", int digits = 6, int period = 30)
        {
            if (string.IsNullOrWhiteSpace(account))
                throw new DomainException(MfaChallengeErrors.NullUserId);

            var issuerEncoded = Uri.EscapeDataString(issuer);
            var accountEncoded = Uri.EscapeDataString(account);
            return $"otpauth://totp/{issuerEncoded}:{accountEncoded}?secret={Base32}&issuer={issuerEncoded}&digits={digits}&period={period}";
        }

        public static bool VerifyCode(string secretBase32, string code, int allowedDriftSteps = 1)
        {
            if (string.IsNullOrWhiteSpace(code) || !RegexCheck.CodeRegex.IsMatch(code))
                return false;
            if (string.IsNullOrWhiteSpace(secretBase32))
                return false;

            var secret = new MfaSecret(secretBase32).ToBytes();
            var currentStep = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 30;
            for (var offset = -allowedDriftSteps; offset <= allowedDriftSteps; offset++)
            {
                if (ComputeTotp(secret, currentStep + offset) == code) return true;
            }
            return false;
        }

        private static string ComputeTotp(byte[] secret, long timeStep)
        {
            var counter = BitConverter.GetBytes(timeStep);
            if (BitConverter.IsLittleEndian) Array.Reverse(counter);

            using var hmac = new HMACSHA1(secret);
            var hash = hmac.ComputeHash(counter);
            var offset = hash[^1] & 0x0f;
            var binary = ((hash[offset] & 0x7f) << 24)
                         | ((hash[offset + 1] & 0xff) << 16)
                         | ((hash[offset + 2] & 0xff) << 8)
                         | (hash[offset + 3] & 0xff);
            return (binary % 1_000_000).ToString("D6");
        }

        private static string EncodeBase32(byte[] bytes)
        {
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            var output = new StringBuilder((bytes.Length * 8 + 4) / 5);
            var buffer = 0;
            var bitsLeft = 0;
            foreach (var value in bytes)
            {
                buffer = (buffer << 8) | value;
                bitsLeft += 8;
                while (bitsLeft >= 5)
                {
                    output.Append(alphabet[(buffer >> (bitsLeft - 5)) & 31]);
                    bitsLeft -= 5;
                }
            }
            if (bitsLeft > 0) output.Append(alphabet[(buffer << (5 - bitsLeft)) & 31]);
            return output.ToString();
        }

        private static byte[] DecodeBase32(string input)
        {
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            var bytes = new List<byte>();
            var buffer = 0;
            var bitsLeft = 0;
            foreach (var c in input.TrimEnd('=').ToUpperInvariant())
            {
                var value = alphabet.IndexOf(c);
                if (value < 0) throw new FormatException("Invalid Base32 secret.");
                buffer = (buffer << 5) | value;
                bitsLeft += 5;
                if (bitsLeft >= 8)
                {
                    bytes.Add((byte)((buffer >> (bitsLeft - 8)) & 255));
                    bitsLeft -= 8;
                }
            }
            return bytes.ToArray();
        }

        private static class RegexCheck
        {
            public static readonly System.Text.RegularExpressions.Regex CodeRegex =
                new(@"^\d{6}$", System.Text.RegularExpressions.RegexOptions.Compiled);
        }

        public override string ToString() => Base32;

        public override bool Equals(object? obj) => obj is MfaSecret s && s.Base32 == Base32;
        public override int GetHashCode() => Base32.GetHashCode();
    }
}
