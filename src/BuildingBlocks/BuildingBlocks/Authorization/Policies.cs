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

            public static class Patients
            {
                public const string View = Prefix + "patients:read";
                public const string Create = Prefix + "patients:create";
                public const string Edit = Prefix + "patients:update";
                public const string Delete = Prefix + "patients:deactivate";
                public const string All = Prefix + "patients:*";
            }

            public static class Agenda
            {
                public const string View = Prefix + "agenda:appointments_read";
                public const string Create = Prefix + "agenda:appointments_create";
                public const string Edit = Prefix + "agenda:availability_update";
                public const string Delete = Prefix + "agenda:appointments_cancel";
                public const string All = Prefix + "agenda:*";
            }

            public static class Sessions
            {
                public const string View = Prefix + "sessions:read";
                public const string Create = Prefix + "sessions:create";
                public const string Edit = Prefix + "sessions:start";
                public const string Delete = Prefix + "sessions:cancel";
                public const string All = Prefix + "sessions:*";
            }

            public static class ClinicalRecords
            {
                public const string View = Prefix + "clinical_records:read";
                public const string Create = Prefix + "clinical_records:create";
                public const string Edit = Prefix + "clinical_records:evolution_autosave";
                public const string Delete = Prefix + "clinical_records:audit_read";
                public const string All = Prefix + "clinical_records:*";
            }

            public static class Notifications
            {
                public const string View = Prefix + "notifications:template_read";
                public const string Create = Prefix + "notifications:template_create";
                public const string Edit = Prefix + "notifications:template_update";
                public const string Delete = Prefix + "notifications:template_delete";
                public const string All = Prefix + "notifications:*";
            }

            public static class OnlineSession
            {
                public const string View = Prefix + "online_session:video_room_read";
                public const string Create = Prefix + "online_session:video_room_upsert";
                public const string Edit = Prefix + "online_session:default_link_upsert";
                public const string Delete = Prefix + "online_session:video_room_clicks_read";
                public const string All = Prefix + "online_session:*";
            }
        }
    }
}
