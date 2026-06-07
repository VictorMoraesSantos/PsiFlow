using Core.Domain.Aggregates;
using Core.Domain.Filters;
using Core.Domain.Repositories;
using Core.Infrastructure.Persistence.Specifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace Core.Infrastructure.Repositories
{
    public abstract class Repository<T, TId, TFilter> : IRepository<T, TId, TFilter>
        where T : class, IBaseEntity<TId>
        where TFilter : IDomainQuery
    {
        protected DbContext Context { get; }
        protected DbSet<T> DbSet { get; }

        protected Repository(DbContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            DbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetById(TId id, CancellationToken cancellationToken = default)
        {
            if (id is null) return null;
            return await DbSet.FindAsync(new object?[] { id }, cancellationToken);
        }

        public virtual async Task<IEnumerable<T?>> GetAll(CancellationToken cancellationToken = default)
        {
            return await DbSet.AsNoTracking().ToListAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<T?>> Find(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            return await DbSet.Where(predicate).ToListAsync(cancellationToken);
        }

        public virtual async Task Create(T entity, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entity);
            await DbSet.AddAsync(entity, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task CreateRange(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entities);
            await DbSet.AddRangeAsync(entities, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task Update(T entity, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entity);

            EntityEntry entry = Context.Entry(entity);
            if (entry.State == EntityState.Detached)
                DbSet.Attach(entity);

            entry.State = EntityState.Modified;
            await Context.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task Delete(T entity, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entity);

            EntityEntry entry = Context.Entry(entity);
            if (entry.State == EntityState.Detached)
                DbSet.Attach(entity);

            entry.State = EntityState.Deleted;
            await Context.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task<(IEnumerable<T> Items, int TotalCount)> FindByFilter(TFilter filter, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(filter);

            var spec = CreateSpecification(filter);

            var totalCount = await SpecificationEvaluator
                .GetCountQuery(DbSet.AsNoTracking(), spec)
                .CountAsync(cancellationToken);

            var items = await SpecificationEvaluator
                .GetQuery(DbSet.AsNoTracking(), spec)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        protected abstract Specification<T, TId> CreateSpecification(TFilter filter);
    }
}
