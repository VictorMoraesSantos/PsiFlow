using Auth.Domain.Aggregates;

namespace Auth.Infrastructure.Persistence.Seeds
{
    public static class PermissionGroupSeed
    {
        public static IReadOnlyCollection<PermissionGroup> GetDefaultGroups()
        {
            var groups = new List<PermissionGroup>
            {
                CreateCrudGroup("dashboard", "Dashboard", "Visão geral da operação clínica"),

                CreateCrudGroup("profile", "Perfil profissional", "Perfil da psicóloga (CRP, bio, especialidades, abordagens, valores, modalidade)"),
                CreateCrudGroup("users", "Usuários", "Gestão de usuários da plataforma"),
                CreateCrudGroup("roles", "Perfis de acesso", "Gestão de perfis de acesso (psychologist, patient, saas_admin)"),
                CreateCrudGroup("permissions", "Permissões", "Gestão de permissões e grupos de permissões"),
                CreateCrudGroup("consents", "Consentimentos", "Aceites versionados de termos de uso e política de privacidade (LGPD)"),
                CreateCrudGroup("audit_logs", "Logs de auditoria", "Trilhas de auditoria de acesso e alteração (LGPD/CFP)"),

                CreateCrudGroup("patients", "Pacientes", "Cadastro e histórico administrativo de pacientes"),
                CreateCrudGroup("patient_invites", "Convites de paciente", "Convites por link enviados a novos pacientes"),

                CreateCrudGroup("availability", "Disponibilidade", "Disponibilidade semanal recorrente da psicóloga"),
                CreateCrudGroup("schedule_blocks", "Bloqueios de agenda", "Bloqueios pontuais (férias, compromissos)"),
                CreateCrudGroup("appointments", "Agendamentos", "Reservas de horário entre psicóloga e paciente"),

                CreateCrudGroup("sessions", "Sessões", "Ciclo de vida da sessão (scheduled, in_progress, completed, no_show, canceled)"),

                CreateCrudGroup("clinical_records", "Prontuários", "Prontuário psicológico (1:1 com paciente) — dado sensível"),
                CreateCrudGroup("evolutions", "Evoluções", "Evoluções clínicas por sessão — dado sensível"),
                CreateCrudGroup("anamnesis", "Anamneses", "Anamnese do paciente — dado sensível"),
                CreateCrudGroup("therapeutic_plans", "Planos terapêuticos", "Plano terapêutico do paciente — dado sensível"),

                CreateCrudGroup("online_sessions", "Sessões online", "Links externos de videochamada (Zoom/Google Meet)"),

                CreateCrudGroup("notifications", "Notificações", "Logs e histórico de notificações enviadas"),
                CreateCrudGroup("notification_preferences", "Preferências de notificação", "Preferências de canal e consentimento por usuário"),

                CreateCrudGroup("reports", "Relatórios", "Relatórios de comparecimento, cancelamentos tardios e ocupação da agenda")
            };
            return groups;
        }

        private static PermissionGroup CreateCrudGroup(string key, string name, string description)
        {
            var group = new PermissionGroup(key, name, description);
            group.AddDefaultCrudPermissions();
            return group;
        }
    }
}
