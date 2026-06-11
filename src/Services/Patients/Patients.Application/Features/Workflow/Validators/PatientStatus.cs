namespace Patients.Application.Features.Workflow;

public static class PatientStatus
{
    public static readonly HashSet<string> AllowedTreatmentStatuses = ["active_treatment", "discharged", "paused", "screening"];
}
