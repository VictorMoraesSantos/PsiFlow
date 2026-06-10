using BuildingBlocks.Results;

namespace Patients.Application.Contracts;

public interface IPatientInviteService
{
    Task<Result> DeactivateAsync(int patientId, string? reason, int tenantId, int userId, CancellationToken cancellationToken);
    Task<Result<object>> ChangeTreatmentStatusAsync(int patientId, string treatmentStatus, string? reason, int tenantId, int userId, CancellationToken cancellationToken);
    Task<Result<object>> GetSessionsSummaryAsync(int patientId, CancellationToken cancellationToken);
    Task<Result<object>> CreateInviteAsync(string email, string? phone, int? patientId, int tenantId, int userId, CancellationToken cancellationToken);
    Task<Result<object>> PreviewInviteAsync(string token, CancellationToken cancellationToken);
    Task<Result<object>> AcceptInviteAsync(string token, int userId, CancellationToken cancellationToken);
    Task<Result> RevokeInviteAsync(int inviteId, int tenantId, CancellationToken cancellationToken);
}
