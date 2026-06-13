using Auth.Application.Authorization;
using Auth.Domain.Entities;

namespace Auth.Infrastructure.Persistence.Seeds
{
    public static class PermissionGroupSeed
    {
        public static IEnumerable<PermissionGroup> DefaultGroups() => new[]
        {
            Build("patients", "Pacientes"),
            Build("agenda", "Agenda"),
            Build("sessions", "Sessoes"),
            Build("clinical_records", "Prontuarios"),
            Build("notifications", "Notificacoes"),
            Build("online_session", "Sessoes Online"),
        };

        public const string AdminEmail = "admin@psiflow.local";
        public const string AdminPassword = "PsiFlow-Admin-2026!";
        public const string AdminRole = "saas_admin";

        public const string PsychologistEmail = "psychologist@psiflow.local";
        public const string PsychologistPassword = "PsiFlow-Psy-2026!";
        public const string PsychologistRole = "psychologist";

        public const string PatientEmail = "patient@psiflow.local";
        public const string PatientPassword = "PsiFlow-Patient-2026!";
        public const string PatientRole = "patient";

        public const string AllPermissionsClaim = "*";

        public static IEnumerable<(string Group, string Action)> DefaultPermissionKeys()
        {
            foreach (var permission in PermissionCatalog.PsychologistPermissions().Concat(PermissionCatalog.PatientPermissions()).Concat(PermissionCatalog.SaasAdminPermissions()).Distinct())
            {
                var parts = permission.Split(':', 2);
                yield return (parts[0], parts[1]);
            }
        }


        private static PermissionGroup Build(string key, string name)
        {
            var group = new PermissionGroup(key, name, $"Permissoes de {name}");
            group.AddDefaultCrudPermissions();
            return group;
        }
    }
}
