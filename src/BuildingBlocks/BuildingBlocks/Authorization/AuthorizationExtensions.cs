using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
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
                o.AddPolicy(Roles.RequirePsychologist, p => p.RequireAuthenticatedUser().RequireRole("psychologist"));
                o.AddPolicy(Roles.RequirePatient, p => p.RequireAuthenticatedUser().RequireRole("patient"));
                o.AddPolicy(Roles.RequireSaasAdmin, p => p.RequireSaasAdmin());
                o.AddPolicy(Roles.RequirePsychologistOrPatient, p => p.RequireAuthenticatedUser().RequireRole("psychologist", "patient"));

                AddPermissionPolicy(o, Permissions.Patients.View);
                AddPermissionPolicy(o, Permissions.Patients.Create);
                AddPermissionPolicy(o, Permissions.Patients.Edit);
                AddPermissionPolicy(o, Permissions.Patients.Delete);

                AddPermissionPolicy(o, Permissions.Agenda.View);
                AddPermissionPolicy(o, Permissions.Agenda.Create);
                AddPermissionPolicy(o, Permissions.Agenda.Edit);
                AddPermissionPolicy(o, Permissions.Agenda.Delete);

                AddPermissionPolicy(o, Permissions.Sessions.View);
                AddPermissionPolicy(o, Permissions.Sessions.Create);
                AddPermissionPolicy(o, Permissions.Sessions.Edit);
                AddPermissionPolicy(o, Permissions.Sessions.Delete);

                AddPermissionPolicy(o, Permissions.ClinicalRecords.View);
                AddPermissionPolicy(o, Permissions.ClinicalRecords.Create);
                AddPermissionPolicy(o, Permissions.ClinicalRecords.Edit);
                AddPermissionPolicy(o, Permissions.ClinicalRecords.Delete);

                AddPermissionPolicy(o, Permissions.Notifications.View);
                AddPermissionPolicy(o, Permissions.Notifications.Create);
                AddPermissionPolicy(o, Permissions.Notifications.Edit);
                AddPermissionPolicy(o, Permissions.Notifications.Delete);

                AddPermissionPolicy(o, Permissions.OnlineSession.View);
                AddPermissionPolicy(o, Permissions.OnlineSession.Create);
                AddPermissionPolicy(o, Permissions.OnlineSession.Edit);
                AddPermissionPolicy(o, Permissions.OnlineSession.Delete);
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
    }

    public sealed class PermissionRequirement : IAuthorizationRequirement
    {
        public string Key { get; }
        public PermissionRequirement(string key) => Key = key;
    }
}
