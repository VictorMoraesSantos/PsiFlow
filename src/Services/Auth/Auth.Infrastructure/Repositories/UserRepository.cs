using Auth.Domain.Entities;
using Auth.Domain.Filters;
using Auth.Domain.Filters.Specifications;
using Auth.Domain.Repositories;
using Auth.Domain.ValueObjects;
using Auth.Infrastructure.Persistence.Data;
using Core.Infrastructure.Persistence.Specifications;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Auth.Infrastructure.Repositories
{
    public class UserRepository(ApplicationDbContext dbContext) : IUserRepository
    {
        public async Task<User?> GetById(UserId id, CancellationToken cancellationToken = default) =>
            await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task<IEnumerable<User?>> GetAll(CancellationToken cancellationToken = default) =>
            await dbContext.Users.AsNoTracking().ToListAsync(cancellationToken);

        public async Task<IEnumerable<User?>> Find(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default) =>
            await dbContext.Users.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

        public async Task<(IEnumerable<User> Items, int TotalCount)> FindByFilter(UserFilter filter, CancellationToken cancellationToken = default)
        {
            var spec = new UserSpecification(filter);
            IQueryable<User> query = dbContext.Users.AsNoTracking();
            IQueryable<User> countQuery = spec.Criteria != null ? query.Where(spec.Criteria) : query;
            int totalCount = await countQuery.CountAsync(cancellationToken);
            IQueryable<User> finalQuery = SpecificationEvaluator.GetQuery(query, spec);
            IEnumerable<User> items = await finalQuery.ToListAsync(cancellationToken);
            return (items, totalCount);
        }

        public async Task Create(User entity, CancellationToken cancellationToken = default) =>
            await dbContext.Users.AddAsync(entity, cancellationToken);

        public async Task CreateRange(IEnumerable<User> entities, CancellationToken cancellationToken = default) =>
            await dbContext.Users.AddRangeAsync(entities, cancellationToken);

        public async Task Update(User entity, CancellationToken cancellationToken = default) =>
            dbContext.Users.Update(entity);

        public async Task Delete(User entity, CancellationToken cancellationToken = default) =>
            dbContext.Users.Remove(entity);

        public async Task<User?> FindByEmail(string email, CancellationToken cancellationToken = default) =>
            await dbContext.Users.FirstOrDefaultAsync(x => x.Email != null && x.Email == email.ToLowerInvariant(), cancellationToken);

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
            await dbContext.SaveChangesAsync(cancellationToken);
    }
}
