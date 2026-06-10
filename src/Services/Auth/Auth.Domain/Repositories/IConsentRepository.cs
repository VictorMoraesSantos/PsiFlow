using Auth.Domain.Aggregates;
using Core.Domain.Repositories;

namespace Auth.Domain.Repositories
{
    public interface IConsentRepository : IRepository<Consent, int>
    {
        Task<Consent?> FindByUserAndVersionAsync(int userId, string termsVersion, string privacyVersion, CancellationToken cancellationToken = default);
    }
}
