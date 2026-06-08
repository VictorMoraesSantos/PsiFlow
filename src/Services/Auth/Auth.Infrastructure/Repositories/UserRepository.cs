using Auth.Domain.Aggregates;
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
    public class UserRepository(ApplicationDbContext _dbContext) : IUserRepository
    {
        public async Task<User?> GetById(UserId id, CancellationToken cancellationToken = default)
        {
            var entity = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            return entity;
        }

        public async Task<IEnumerable<User?>> GetAll(CancellationToken cancellationToken = default)
        {
            var entities = await _dbContext.Users.AsNoTracking().ToListAsync(cancellationToken);
            return entities;
        }

        public async Task<IEnumerable<User?>> Find(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var entities = await _dbContext.Users.AsNoTracking().Where(predicate).ToListAsync();
            return entities;
        }

        public async Task<(IEnumerable<User> Items, int TotalCount)> FindByFilter(UserFilter filter, CancellationToken cancellationToken = default)
        {
            var spec = new UserSpecification(filter);
            IQueryable<User> query = _dbContext.Users.AsNoTracking();
            IQueryable<User> countQuery = spec.Criteria != null ? query.Where(spec.Criteria) : query;
            int totalCount = await countQuery.CountAsync(cancellationToken);
            IQueryable<User> finalQuery = SpecificationEvaluator.GetQuery(query, spec);
            IEnumerable<User> items = await finalQuery.ToListAsync(cancellationToken);
            return (items, totalCount);
        }

        public async Task Create(User entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.Users.AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync();
        }

        public async Task CreateRange(IEnumerable<User> entities, CancellationToken cancellationToken = default)
        {
            await _dbContext.Users.AddRangeAsync(entities, cancellationToken);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(User entity, CancellationToken cancellationToken = default)
        {
            _dbContext.Users.Update(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(User entity, CancellationToken cancellationToken = default)
        {
            _dbContext.Users.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
