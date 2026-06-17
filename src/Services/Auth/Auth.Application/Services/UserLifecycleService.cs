using Auth.Application.Contracts;
using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using Auth.Domain.ValueObjects;
using BuildingBlocks.Results;

namespace Auth.Application.Services
{
    public class UserLifecycleService : IUserLifecycleService
    {
        private readonly IUserRepository _userRepository;

        public UserLifecycleService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result> BeginLoginAsync(User user, CancellationToken cancellationToken = default)
        {
            user.BeginLogin();
            await _userRepository.Update(user, cancellationToken);

            var success = Result.Success();
            return success;
        }

        public async Task<Result> AttachTenantAsync(User user, UserId tenantId, CancellationToken cancellationToken = default)
        {
            user.AttachTenant(new TenantId(tenantId));
            await _userRepository.Update(user, cancellationToken);

            var success = Result.Success();
            return success;
        }
    }
}
