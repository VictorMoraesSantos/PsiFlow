using BuildingBlocks.Results;

namespace Auth.Application.Contracts
{
    public interface IEmailVerificationService
    {
        Task<Result<string>> RequestAsync(string email, CancellationToken cancellationToken = default);
        Task<Result> VerifyAsync(string email, string token, CancellationToken cancellationToken = default);
    }
}
