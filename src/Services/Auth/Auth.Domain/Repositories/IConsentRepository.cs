using Auth.Domain.Entities;
using Auth.Domain.ValueObjects;
using Core.Domain.Repositories;

namespace Auth.Domain.Repositories
{
    public interface IConsentRepository : IRepository<Consent, ConsentId>
    {
        Task<Consent?> FindByUserAndVersion(int userId, string termsVersion, string privacyVersion, CancellationToken cancellationToken = default);
    }
}
