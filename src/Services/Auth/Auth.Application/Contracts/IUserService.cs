using Auth.Application.DTOs.Auth;
using Auth.Application.DTOs.Users;
using Auth.Domain.Entities;
using Auth.Domain.Filters;
using Auth.Domain.ValueObjects;
using BuildingBlocks.Results;
using Core.Application.Interfaces;

namespace Auth.Application.Contracts
{
    public interface IUserService :
        IReadService<UserDTO, int, UserFilter>,
        IUpdateService<UpdateUserDTO>,
        IDeleteService<int>
    {
        Task<Result<UserDTO>> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<Result> UpdateCurrentUserProfileAsync(int userId, UpdateCurrentUserProfileDTO dto, CancellationToken cancellationToken = default);
        Task<Result> DeleteCurrentUserAsync(int userId, CancellationToken cancellationToken = default);
        Task<Result<MeResponse>> GetMeAsync(int userId, CancellationToken cancellationToken = default);
        Task<Result> BeginLoginAsync(User user, CancellationToken cancellationToken = default);
        Task<Result> AttachTenantAsync(User user, UserId tenantId, CancellationToken cancellationToken = default);
        Task<Result<RegisterResult>> RegisterAsync(RegisterDTO dto, CancellationToken cancellationToken = default);
    }
}
