using Auth.Application.Authorization;
using Auth.Application.Contracts;
using Auth.Domain.Entities;
using Auth.Domain.ValueObjects;
using BuildingBlocks.Results;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Auth.Application.Services
{
    public class PermissionAssignmentService : IPermissionAssignmentService
    {
        private readonly UserManager<User> _userManager;

        public PermissionAssignmentService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result> AssignDefaultAsync(User user, CancellationToken cancellationToken = default)
        {
            var permissions = user.Role switch
            {
                UserRole.SaasAdmin => new[] { "*" },
                UserRole.Psychologist => PermissionCatalog.PsychologistPermissions(),
                UserRole.Patient => PermissionCatalog.PatientPermissions(),
                _ => Array.Empty<string>()
            };

            if (permissions.Length == 0)
                return Result.Success();

            if (permissions.Length == 1 && permissions[0] == "*")
            {
                var addResult = await _userManager.AddClaimAsync(user, new Claim("permission", "*"));
                if (addResult.Succeeded)
                    return Result.Success();

                var error = Error.Failure(string.Join("; ", addResult.Errors.Select(e => e.Description)));
                return Result.Failure(error);
            }

            foreach (var permission in permissions)
            {
                var addResult = await _userManager.AddClaimAsync(user, new Claim("permission", permission));
                if (!addResult.Succeeded)
                {
                    var error = Error.Failure(string.Join("; ", addResult.Errors.Select(e => e.Description)));
                    return Result.Failure(error);
                }
            }

            return Result.Success();
        }
    }
}
