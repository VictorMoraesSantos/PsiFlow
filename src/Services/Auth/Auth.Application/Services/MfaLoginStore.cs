using System.Collections.Concurrent;

namespace Auth.Application.Services
{
    public sealed class MfaLoginStore
    {
        private readonly ConcurrentDictionary<string, MfaLoginEntry> _entries = new();
        private readonly TimeSpan _ttl = TimeSpan.FromMinutes(5);

        public string Create(int userId, int challengeId)
        {
            var token = Guid.NewGuid().ToString("N");
            _entries[token] = new MfaLoginEntry(userId, challengeId, DateTime.UtcNow);
            Sweep();
            return token;
        }

        public bool TryConsume(string token, out MfaLoginEntry entry)
        {
            if (!_entries.TryRemove(token, out entry!)) return false;
            return DateTime.UtcNow - entry.CreatedAt <= _ttl;
        }

        private void Sweep()
        {
            var cutoff = DateTime.UtcNow - _ttl;
            foreach (var key in _entries.Keys)
                if (_entries.TryGetValue(key, out var e) && e.CreatedAt < cutoff)
                    _entries.TryRemove(key, out _);
        }

        public sealed record MfaLoginEntry(int UserId, int ChallengeId, DateTime CreatedAt);
    }
}
