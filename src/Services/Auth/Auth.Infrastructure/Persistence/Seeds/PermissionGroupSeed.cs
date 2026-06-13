using Auth.Domain.Entities;
using Auth.Domain.ValueObjects;

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
            foreach (var group in new[] { "patients", "agenda", "sessions", "clinical_records", "notifications", "online_session" })
            {
                yield return (group, "view");
                yield return (group, "create");
                yield return (group, "edit");
                yield return (group, "delete");
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
