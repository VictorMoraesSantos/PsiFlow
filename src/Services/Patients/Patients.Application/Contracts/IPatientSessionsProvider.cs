using BuildingBlocks.Results;

namespace Patients.Application.Contracts;

public interface IPatientSessionsProvider
{
    Task<Result<IReadOnlyCollection<PatientSessionHistoryDTO>>> GetPatientSessionsAsync(int patientId, int tenantId, CancellationToken cancellationToken);
}

public sealed record PatientSessionHistoryDTO(
    int Id,
    int AppointmentId,
    int PatientId,
    int PsychologistId,
    DateTime StartsAt,
    DateTime EndsAt,
    string Status,
    string Modality,
    string PaymentStatus,
    string? OnlineSessionLink);
