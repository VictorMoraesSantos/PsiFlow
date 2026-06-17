using Auth.Application.DTOs.Auth;
using BuildingBlocks.Results;

namespace Auth.Application.Contracts
{
    public interface IConsentService
    {
        Task<Result> RecordAsync(int userId, ConsentDTO dto, CancellationToken cancellationToken = default);
    }
}
