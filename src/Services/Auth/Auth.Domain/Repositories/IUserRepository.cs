using Auth.Domain.Entities;
using Auth.Domain.Filters;
using Core.Domain.Repositories;

namespace Auth.Domain.Repositories
{
    public interface IUserRepository : IRepository<User, int, UserFilter>
    {
        Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<User?> FindByRefreshTokenHashAsync(string refreshTokenHash, CancellationToken cancellationToken = default);
    }
}
