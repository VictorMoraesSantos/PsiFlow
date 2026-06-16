namespace Auth.Application.Authorization
{
    public static class PermissionCatalog
    {
        public static string[] SaasAdminPermissions() => new[]
        {
            "auth:me_read", "auth:consent_accept", "auth:password_change", "auth:session_logout",
            "auth:users_read", "auth:users_suspend", "auth:roles_manage", "auth:permissions_manage", "auth:audit_read",
            "notifications:template_create", "notifications:template_read", "notifications:template_update",
            "notifications:template_delete", "notifications:template_version_create",
            "notifications:logs_read", "notifications:test_email_send", "notifications:retry_send", "notifications:schedule_reminder"
        };

        public static string[] PsychologistPermissions() => new[]
        {
            "auth:me_read", "auth:consent_accept", "auth:password_change",
            "auth:mfa_setup", "auth:mfa_verify", "auth:session_logout",

            "patients:create", "patients:list", "patients:read", "patients:update", "patients:deactivate",
            "patients:treatment_status_update", "patients:invite_create", "patients:invite_revoke",
            "patients:emergency_contact_manage", "patients:legal_responsible_manage",
            "patients:admin_notes_manage", "patients:sessions_summary_read",

            "agenda:availability_create", "agenda:availability_read", "agenda:availability_update", "agenda:availability_delete",
            "agenda:blocks_create", "agenda:blocks_read", "agenda:blocks_delete", "agenda:available_slots_read",
            "agenda:appointments_create", "agenda:appointments_read", "agenda:appointments_cancel",
            "agenda:calendar_day_read", "agenda:calendar_week_read", "agenda:calendar_month_read",

            "sessions:read", "sessions:patient_history_read", "sessions:start", "sessions:complete",
            "sessions:no_show", "sessions:cancel", "sessions:status_history_read",
            "sessions:payment_mark_received", "sessions:payment_read", "sessions:receipt_send",

            "clinical_records:create", "clinical_records:read",
            "clinical_records:anamnesis_read", "clinical_records:anamnesis_autosave",
            "clinical_records:anamnesis_publish", "clinical_records:anamnesis_versions_read",
            "clinical_records:evolution_read", "clinical_records:evolution_autosave",
            "clinical_records:evolution_publish", "clinical_records:evolution_versions_read",
            "clinical_records:audit_read",

            "notifications:logs_read",

            "online_session:video_room_upsert", "online_session:video_room_read",
            "online_session:video_room_click", "online_session:video_room_clicks_read",
            "online_session:default_link_upsert", "online_session:default_link_read",
            "online_session:instructions_update"
        };

        public static string[] PatientPermissions() => new[]
        {
            "auth:me_read", "auth:consent_accept", "auth:password_change", "auth:session_logout",

            "patients:read", "patients:update", "patients:invite_accept",
            "patients:emergency_contact_manage", "patients:legal_responsible_manage",
            "patients:sessions_summary_read",

            "agenda:availability_read", "agenda:available_slots_read",
            "agenda:appointments_read", "agenda:appointments_cancel",
            "agenda:calendar_day_read", "agenda:calendar_week_read", "agenda:calendar_month_read",

            "sessions:read", "sessions:patient_history_read",
            "sessions:cancel", "sessions:payment_read",

            "online_session:video_room_read", "online_session:video_room_click"
        };
    }
}
