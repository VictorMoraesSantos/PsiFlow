namespace Patients.Application.Features.Workflow;

public static class PatientStatus
{
    public const string AdministrativeActive = "active";
    public const string AdministrativeInactive = "inactive";
    public const string Screening = "screening";
    public const string InTreatment = "in_treatment";
    public const string Paused = "paused";
    public const string Discharged = "discharged";

    public static readonly HashSet<string> AllowedAdministrativeStatuses = [AdministrativeActive, AdministrativeInactive];
    public static readonly HashSet<string> AllowedTreatmentStatuses = [Screening, InTreatment, Paused, Discharged];

    public static bool CanTransitionTreatment(string from, string to) => from == to || (from, to) switch
    {
        (Screening, InTreatment) => true,
        (Screening, Paused) => true,
        (InTreatment, Paused) => true,
        (InTreatment, Discharged) => true,
        (Paused, InTreatment) => true,
        (Paused, Discharged) => true,
        _ => false
    };
}
