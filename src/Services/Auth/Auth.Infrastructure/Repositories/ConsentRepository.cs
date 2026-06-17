using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using Auth.Domain.ValueObjects;
using Auth.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Auth.Infrastructure.Repositories
{
    public class ConsentRepository(ApplicationDbContext dbContext) : IConsentRepository
    {
        public async Task<Consent?> GetById(ConsentId id, CancellationToken cancellationToken = default)
        {
            var consent = await dbContext.Consents.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            return consent;
        }

        public async Task<IEnumerable<Consent?>> GetAll(CancellationToken cancellationToken = default)
        {
            var consents = await dbContext.Consents.AsNoTracking().ToListAsync(cancellationToken);
            return consents;
        }

        public async Task<IEnumerable<Consent?>> Find(Expression<Func<Consent, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var consents = await dbContext.Consents.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
            return consents;
        }

        public async Task Create(Consent entity, CancellationToken cancellationToken = default)
        {
            await dbContext.Consents.AddAsync(entity, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateRange(IEnumerable<Consent> entities, CancellationToken cancellationToken = default)
        {
            await dbContext.Consents.AddRangeAsync(entities, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task Update(Consent entity, CancellationToken cancellationToken = default)
        {
            dbContext.Consents.Update(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(Consent entity, CancellationToken cancellationToken = default)
        {
            dbContext.Consents.Remove(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<Consent?> FindByUserAndVersion(int userId, string termsVersion, string privacyVersion, CancellationToken cancellationToken = default)
        {
            var consent = await dbContext.Consents.FirstOrDefaultAsync(x => x.UserId == userId && x.TermsVersion == termsVersion && x.PrivacyVersion == privacyVersion, cancellationToken);
            return consent;
        }
    }
}
