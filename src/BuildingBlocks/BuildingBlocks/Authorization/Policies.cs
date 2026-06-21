namespace BuildingBlocks.Authorization
{
    public static class Policies
    {
        public static class Roles
        {
            public const string RequirePsychologist = "RequirePsychologist";
            public const string RequirePatient = "RequirePatient";
            public const string RequireSaasAdmin = "RequireSaasAdmin";
            public const string RequirePsychologistOrPatient = "RequirePsychologistOrPatient";
            public const string RequireAuthenticated = "RequireAuthenticated";
        }

        public static class Permissions
        {
            public const string Prefix = "Perm:";

            public static string Key(string permission) => Prefix + permission;

            public static class Auth
            {
                public const string MeRead = Prefix + "auth:me_read";
                public const string ConsentAccept = Prefix + "auth:consent_accept";
                public const string PasswordChange = Prefix + "auth:password_change";
                public const string SessionLogout = Prefix + "auth:session_logout";
                public const string UsersRead = Prefix + "auth:users_read";
                public const string UsersSuspend = Prefix + "auth:users_suspend";
                public const string RolesManage = Prefix + "auth:roles_manage";
                public const string PermissionsManage = Prefix + "auth:permissions_manage";
                public const string AuditRead = Prefix + "auth:audit_read";
            }

            public static class Patients
            {
                public const string List = Prefix + "patients:list";
                public const string Read = Prefix + "patients:read";
                public const string Create = Prefix + "patients:create";
                public const string Update = Prefix + "patients:update";
                public const string Deactivate = Prefix + "patients:deactivate";
                public const string TreatmentStatusUpdate = Prefix + "patients:treatment_status_update";
                public const string InviteCreate = Prefix + "patients:invite_create";
                public const string InviteRevoke = Prefix + "patients:invite_revoke";
                public const string InviteAccept = Prefix + "patients:invite_accept";
                public const string EmergencyContactManage = Prefix + "patients:emergency_contact_manage";
                public const string LegalResponsibleManage = Prefix + "patients:legal_responsible_manage";
                public const string AdminNotesManage = Prefix + "patients:admin_notes_manage";
                public const string SessionsSummaryRead = Prefix + "patients:sessions_summary_read";
                public const string All = Prefix + "patients:*";

                public const string View = Read;
                public const string Edit = Update;
                public const string Delete = Deactivate;
            }

            public static class Agenda
            {
                public const string AvailabilityCreate = Prefix + "agenda:availability_create";
                public const string AvailabilityRead = Prefix + "agenda:availability_read";
                public const string AvailabilityUpdate = Prefix + "agenda:availability_update";
                public const string AvailabilityDelete = Prefix + "agenda:availability_delete";
                public const string BlocksCreate = Prefix + "agenda:blocks_create";
                public const string BlocksRead = Prefix + "agenda:blocks_read";
                public const string BlocksDelete = Prefix + "agenda:blocks_delete";
                public const string AvailableSlotsRead = Prefix + "agenda:available_slots_read";
                public const string AppointmentsCreate = Prefix + "agenda:appointments_create";
                public const string AppointmentsRead = Prefix + "agenda:appointments_read";
                public const string AppointmentsCancel = Prefix + "agenda:appointments_cancel";
                public const string CalendarDayRead = Prefix + "agenda:calendar_day_read";
                public const string CalendarWeekRead = Prefix + "agenda:calendar_week_read";
                public const string CalendarMonthRead = Prefix + "agenda:calendar_month_read";
                public const string All = Prefix + "agenda:*";

                public const string View = AppointmentsRead;
                public const string Create = AppointmentsCreate;
                public const string Edit = AvailabilityUpdate;
                public const string Delete = AppointmentsCancel;
            }

            public static class Sessions
            {
                public const string Read = Prefix + "sessions:read";
                public const string PatientHistoryRead = Prefix + "sessions:patient_history_read";
                public const string Start = Prefix + "sessions:start";
                public const string Complete = Prefix + "sessions:complete";
                public const string NoShow = Prefix + "sessions:no_show";
                public const string Cancel = Prefix + "sessions:cancel";
                public const string StatusHistoryRead = Prefix + "sessions:status_history_read";
                public const string PaymentMarkReceived = Prefix + "sessions:payment_mark_received";
                public const string PaymentRead = Prefix + "sessions:payment_read";
                public const string ReceiptSend = Prefix + "sessions:receipt_send";
                public const string All = Prefix + "sessions:*";

                public const string View = Read;
                public const string Create = Start;
                public const string Edit = Start;
                public const string Delete = Cancel;
            }

            public static class ClinicalRecords
            {
                public const string Read = Prefix + "clinical_records:read";
                public const string Create = Prefix + "clinical_records:create";
                public const string AnamnesisRead = Prefix + "clinical_records:anamnesis_read";
                public const string AnamnesisAutosave = Prefix + "clinical_records:anamnesis_autosave";
                public const string AnamnesisPublish = Prefix + "clinical_records:anamnesis_publish";
                public const string AnamnesisVersionsRead = Prefix + "clinical_records:anamnesis_versions_read";
                public const string EvolutionRead = Prefix + "clinical_records:evolution_read";
                public const string EvolutionAutosave = Prefix + "clinical_records:evolution_autosave";
                public const string EvolutionPublish = Prefix + "clinical_records:evolution_publish";
                public const string EvolutionVersionsRead = Prefix + "clinical_records:evolution_versions_read";
                public const string AuditRead = Prefix + "clinical_records:audit_read";
                public const string All = Prefix + "clinical_records:*";

                public const string View = Read;
                public const string Edit = EvolutionAutosave;
                public const string Delete = AuditRead;
            }

            public static class Notifications
            {
                public const string TemplateCreate = Prefix + "notifications:template_create";
                public const string TemplateRead = Prefix + "notifications:template_read";
                public const string TemplateUpdate = Prefix + "notifications:template_update";
                public const string TemplateDelete = Prefix + "notifications:template_delete";
                public const string TemplateVersionCreate = Prefix + "notifications:template_version_create";
                public const string LogsRead = Prefix + "notifications:logs_read";
                public const string TestEmailSend = Prefix + "notifications:test_email_send";
                public const string RetrySend = Prefix + "notifications:retry_send";
                public const string ScheduleReminder = Prefix + "notifications:schedule_reminder";
                public const string All = Prefix + "notifications:*";

                public const string View = TemplateRead;
                public const string Create = TemplateCreate;
                public const string Edit = TemplateUpdate;
                public const string Delete = TemplateDelete;
            }

            public static class OnlineSession
            {
                public const string VideoRoomUpsert = Prefix + "online_session:video_room_upsert";
                public const string VideoRoomRead = Prefix + "online_session:video_room_read";
                public const string VideoRoomClick = Prefix + "online_session:video_room_click";
                public const string VideoRoomClicksRead = Prefix + "online_session:video_room_clicks_read";
                public const string DefaultLinkUpsert = Prefix + "online_session:default_link_upsert";
                public const string DefaultLinkRead = Prefix + "online_session:default_link_read";
                public const string InstructionsUpdate = Prefix + "online_session:instructions_update";
                public const string All = Prefix + "online_session:*";

                public const string View = VideoRoomRead;
                public const string Create = VideoRoomUpsert;
                public const string Edit = VideoRoomUpsert;
                public const string Delete = VideoRoomClicksRead;
            }
        }
    }
}
