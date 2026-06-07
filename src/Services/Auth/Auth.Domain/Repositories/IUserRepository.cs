using Auth.Domain.Entities;
using Auth.Domain.Filters;
using Auth.Domain.ValueObjects;
using Core.Domain.Repositories;

namespace Auth.Domain.Repositories
{
    public interface IUserRepository : IRepository<User, UserId, UserFilter>
    { }
}
