using Microsoft.AspNetCore.Authorization;

namespace BuildingBlocks.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User.IsInRole("saas_admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var hasClaim = context.User.Claims.Any(c =>
                c.Type == AuthorizationExtensions.PermissionClaimType &&
                (c.Value == requirement.Key || c.Value == SuperAdminClaimValue || c.Value == $"{requirement.Key.Split('.').FirstOrDefault()}.*"));

            if (hasClaim) context.Succeed(requirement);
            return Task.CompletedTask;
        }

        private const string SuperAdminClaimValue = "*";
    }
}
