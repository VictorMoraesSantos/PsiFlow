using Auth.Domain.Entities;
using BuildingBlocks.Results;

namespace Auth.Application.Contracts
{
    public interface ICredentialService
    {
        Task<Result<User>> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);
    }
}
