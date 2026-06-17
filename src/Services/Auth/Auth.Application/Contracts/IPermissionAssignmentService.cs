using Auth.Domain.Entities;
using BuildingBlocks.Results;

namespace Auth.Application.Contracts
{
    public interface IPermissionAssignmentService
    {
        Task<Result> AssignDefaultAsync(User user, CancellationToken cancellationToken = default);
    }
}
