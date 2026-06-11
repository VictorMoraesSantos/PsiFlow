using Auth.Domain.Entities;
using Auth.Domain.Enums;

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

        private static PermissionGroup Build(string key, string name)
        {
            var group = new PermissionGroup(key, name, $"Permissoes de {name}");
            group.AddDefaultCrudPermissions();
            return group;
        }
    }
}
