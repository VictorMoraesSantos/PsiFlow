using Auth.Domain.Entities;
using BuildingBlocks.Results;

namespace Auth.Application.Contracts
{
    public interface ICredentialService
    {
        Task<Result<AuthenticatedUser>> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);
    }

    public sealed record AuthenticatedUser(User User, bool RequiresMfa);
}
