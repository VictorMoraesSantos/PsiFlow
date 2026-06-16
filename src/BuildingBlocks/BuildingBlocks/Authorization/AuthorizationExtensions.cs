using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using static BuildingBlocks.Authorization.Policies;

namespace BuildingBlocks.Authorization
{
    public static class AuthorizationExtensions
    {
        public const string PermissionClaimType = "permission";
        public const string SuperAdminClaimValue = "*";

        public static IServiceCollection AddPsiFlowAuthorization(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
            services.AddAuthorization(o =>
            {
                o.AddPolicy(Roles.RequireAuthenticated, p => p.RequireAuthenticatedUser());
                o.AddPolicy(Roles.RequirePsychologist, p => p.RequireAuthenticatedUser().RequireRole("psychologist", "saas_admin"));
                o.AddPolicy(Roles.RequirePatient, p => p.RequireAuthenticatedUser().RequireRole("patient"));
                o.AddPolicy(Roles.RequireSaasAdmin, p => p.RequireSaasAdmin());
                o.AddPolicy(Roles.RequirePsychologistOrPatient, p => p.RequireAuthenticatedUser().RequireRole("psychologist", "patient", "saas_admin"));

                foreach (var permission in DiscoverAllPermissions())
                    AddPermissionPolicy(o, permission);
            });
            return services;
        }

        public static AuthorizationPolicyBuilder RequireSaasAdmin(this AuthorizationPolicyBuilder builder)
        {
            return builder.RequireAuthenticatedUser()
                .RequireAssertion(ctx => ctx.User.IsInRole("saas_admin"));
        }

        private static void AddPermissionPolicy(AuthorizationOptions options, string policyName)
        {
            var key = policyName.StartsWith(Permissions.Prefix)
                ? policyName[Permissions.Prefix.Length..]
                : policyName;

            options.AddPolicy(policyName, p => p.RequireAuthenticatedUser().Requirements.Add(new PermissionRequirement(key)));
        }

        private static IEnumerable<string> DiscoverAllPermissions()
        {
            var groups = typeof(Permissions).GetNestedTypes(BindingFlags.Public | BindingFlags.Static);
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var group in groups)
            {
                var fields = group.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                foreach (var field in fields)
                {
                    if (!field.IsLiteral || field.IsInitOnly) continue;
                    if (field.FieldType != typeof(string)) continue;
                    var value = field.GetRawConstantValue() as string;
                    if (string.IsNullOrWhiteSpace(value)) continue;
                    if (!value.StartsWith(Permissions.Prefix, StringComparison.Ordinal)) continue;
                    if (seen.Add(value))
                        yield return value;
                }
            }
        }
    }

    public sealed class PermissionRequirement : IAuthorizationRequirement
    {
        public string Key { get; }
        public PermissionRequirement(string key) => Key = key;
    }
}
