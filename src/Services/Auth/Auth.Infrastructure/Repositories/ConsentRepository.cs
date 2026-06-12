using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using Auth.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Auth.Infrastructure.Repositories
{
    public class ConsentRepository(ApplicationDbContext dbContext) : IConsentRepository
    {
        public async Task<Consent?> GetById(int id, CancellationToken cancellationToken = default) =>
            await dbContext.Consents.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task<IEnumerable<Consent?>> GetAll(CancellationToken cancellationToken = default) =>
            await dbContext.Consents.AsNoTracking().ToListAsync(cancellationToken);

        public async Task<IEnumerable<Consent?>> Find(Expression<Func<Consent, bool>> predicate, CancellationToken cancellationToken = default) =>
            await dbContext.Consents.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

        public async Task Create(Consent entity, CancellationToken cancellationToken = default) =>
            await dbContext.Consents.AddAsync(entity, cancellationToken);

        public async Task CreateRange(IEnumerable<Consent> entities, CancellationToken cancellationToken = default) =>
            await dbContext.Consents.AddRangeAsync(entities, cancellationToken);

        public async Task Update(Consent entity, CancellationToken cancellationToken = default) =>
            dbContext.Consents.Update(entity);

        public async Task Delete(Consent entity, CancellationToken cancellationToken = default) =>
            dbContext.Consents.Remove(entity);

        public async Task<Consent?> FindByUserAndVersion(int userId, string termsVersion, string privacyVersion, CancellationToken cancellationToken = default) =>
            await dbContext.Consents.FirstOrDefaultAsync(x => x.UserId == userId && x.TermsVersion == termsVersion && x.PrivacyVersion == privacyVersion, cancellationToken);

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
            await dbContext.SaveChangesAsync(cancellationToken);
    }
}
