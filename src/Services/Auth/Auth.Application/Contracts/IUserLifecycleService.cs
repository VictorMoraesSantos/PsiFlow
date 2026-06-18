using Auth.Domain.Entities;
using Auth.Domain.ValueObjects;
using BuildingBlocks.Results;

namespace Auth.Application.Contracts
{
    public interface IUserLifecycleService
    {
        Task<Result> BeginLoginAsync(User user, CancellationToken cancellationToken = default);
        Task<Result> AttachTenantAsync(User user, UserId tenantId, CancellationToken cancellationToken = default);
    }
}
