using Auth.Application.DTOs.Users;
using Auth.Domain.Filters;
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
    }
}
