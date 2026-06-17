using Auth.Application.Contracts;
using Auth.Application.DTOs.Auth;
using Auth.Domain.Entities;
using Auth.Domain.Errors;
using Auth.Domain.Repositories;
using Auth.Domain.ValueObjects;
using BuildingBlocks.Results;
using Microsoft.AspNetCore.Identity;

namespace Auth.Application.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;

        public UserProfileService(IUserRepository userRepository, UserManager<User> userManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
        }

        public async Task<Result<MeResponse>> GetMeAsync(int userId, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetById(new UserId(userId), cancellationToken);
            if (user is null)
            {
                var result = Result.Failure<MeResponse>(UserErrors.NotFound(userId));
                return result;
            }

            var roles = await _userManager.GetRolesAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);
            var permissions = claims
                .Where(c => c.Type == "permission")
                .Select(c => c.Value)
                .OrderBy(v => v, StringComparer.Ordinal)
                .ToArray();

            var response = new MeResponse(
                user.Id.Value,
                user.TenantId.Value,
                user.Email ?? string.Empty,
                user.Role,
                user.Name.FullName,
                user.IsActive,
                user.EmailConfirmed,
                user.IsMfaEnabled,
                permissions,
                roles.ToArray());
            var success = Result.Success(response);
            return success;
        }
    }
}
