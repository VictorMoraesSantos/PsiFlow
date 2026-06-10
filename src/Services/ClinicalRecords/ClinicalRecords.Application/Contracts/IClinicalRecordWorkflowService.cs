using BuildingBlocks.Results;

namespace ClinicalRecords.Application.Contracts;

public interface IClinicalRecordWorkflowService
{
    Task<Result<object>> GetRecordByPatientAsync(int patientId, int tenantId, CancellationToken ct);
    Task<Result<object>> CreateRecordAsync(int patientId, int tenantId, string? name, CancellationToken ct);
    Task<Result<object>> GetRecordAsync(int recordId, int tenantId, CancellationToken ct);
    Task<Result<object>> GetAnamnesisAsync(int recordId, int tenantId, CancellationToken ct);
    Task<Result> AutosaveAnamnesisAsync(int recordId, int tenantId, string? ciphertext, string? nonce, string? tag, CancellationToken ct);
    Task<Result<object>> PublishAnamnesisVersionAsync(int recordId, int tenantId, int userId, string? reason, CancellationToken ct);
    Task<Result<object>> GetEvolutionAsync(int sessionId, int tenantId, CancellationToken ct);
    Task<Result> AutosaveEvolutionAsync(int sessionId, int tenantId, string? ciphertext, string? nonce, string? tag, CancellationToken ct);
    Task<Result<object>> PublishEvolutionVersionAsync(int sessionId, int tenantId, int userId, string? reason, CancellationToken ct);
    Task<Result<object>> GetEvolutionVersionsAsync(int sessionId, int tenantId, CancellationToken ct);
    Task<Result<object>> GetAuditLogAsync(int recordId, int tenantId, CancellationToken ct);
}
