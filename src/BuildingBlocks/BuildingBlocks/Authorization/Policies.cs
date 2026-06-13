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
                public const string View = Prefix + "patients.view";
                public const string Create = Prefix + "patients.create";
                public const string Edit = Prefix + "patients.edit";
                public const string Delete = Prefix + "patients.delete";
                public const string All = Prefix + "patients.*";
            }

            public static class Agenda
            {
                public const string View = Prefix + "agenda.view";
                public const string Create = Prefix + "agenda.create";
                public const string Edit = Prefix + "agenda.edit";
                public const string Delete = Prefix + "agenda.delete";
                public const string All = Prefix + "agenda.*";
            }

            public static class Sessions
            {
                public const string View = Prefix + "sessions.view";
                public const string Create = Prefix + "sessions.create";
                public const string Edit = Prefix + "sessions.edit";
                public const string Delete = Prefix + "sessions.delete";
                public const string All = Prefix + "sessions.*";
            }

            public static class ClinicalRecords
            {
                public const string View = Prefix + "clinical_records.view";
                public const string Create = Prefix + "clinical_records.create";
                public const string Edit = Prefix + "clinical_records.edit";
                public const string Delete = Prefix + "clinical_records.delete";
                public const string All = Prefix + "clinical_records.*";
            }

            public static class Notifications
            {
                public const string View = Prefix + "notifications.view";
                public const string Create = Prefix + "notifications.create";
                public const string Edit = Prefix + "notifications.edit";
                public const string Delete = Prefix + "notifications.delete";
                public const string All = Prefix + "notifications.*";
            }

            public static class OnlineSession
            {
                public const string View = Prefix + "online_session.view";
                public const string Create = Prefix + "online_session.create";
                public const string Edit = Prefix + "online_session.edit";
                public const string Delete = Prefix + "online_session.delete";
                public const string All = Prefix + "online_session.*";
            }
        }
    }
}
