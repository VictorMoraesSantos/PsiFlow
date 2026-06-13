using BuildingBlocks.Results;
using ClinicalRecords.Application.Contracts;
using ClinicalRecords.Domain.Entities;
using ClinicalRecords.Domain.Repositories;

namespace ClinicalRecords.Application.Services;

public sealed class ClinicalRecordWorkflowService(
    IMedicalRecordRepository medicalRecords,
    IAnamnesisRepository anamneses,
    IAnamnesisVersionRepository anamnesisVersions,
    IEvolutionRepository evolutions,
    IEvolutionVersionRepository evolutionVersions,
    IClinicalAuditLogRepository auditLogs) : IClinicalRecordWorkflowService
{
    public async Task<Result<object>> GetRecordByPatientAsync(int patientId, int tenantId, CancellationToken ct) => await OkOrNotFound(medicalRecords.GetByPatientAndTenantAsync(patientId, tenantId, ct));
    public async Task<Result<object>> CreateRecordAsync(int patientId, int tenantId, string? name, CancellationToken ct) { var r = new MedicalRecord { TenantId = tenantId, PatientId = patientId, Name = string.IsNullOrWhiteSpace(name) ? $"Patient {patientId}" : name }; await medicalRecords.Create(r, ct); return Result.Success<object>(r); }
    public async Task<Result<object>> GetRecordAsync(int recordId, int tenantId, CancellationToken ct) => await OkOrNotFound(medicalRecords.GetByIdAndTenantAsync(recordId, tenantId, ct));
    public async Task<Result<object>> GetAnamnesisAsync(int recordId, int tenantId, CancellationToken ct) { var a = await GetOrCreateAnamnesis(recordId, tenantId, ct); return Result.Success<object>(a); }
    public async Task<Result> AutosaveAnamnesisAsync(int recordId, int tenantId, string? ciphertext, string? nonce, string? tag, CancellationToken ct) { var a = await GetOrCreateAnamnesis(recordId, tenantId, ct); a.DraftCiphertext = ciphertext; a.DraftNonce = nonce; a.DraftTag = tag; a.DraftUpdatedAt = DateTime.UtcNow; await anamneses.Update(a, ct); return Result.Success(); }
    public async Task<Result<object>> PublishAnamnesisVersionAsync(int recordId, int tenantId, int userId, string? reason, CancellationToken ct) { var a = await GetOrCreateAnamnesis(recordId, tenantId, ct); var n = await anamnesisVersions.CountByAnamnesisAsync(a.Id, ct) + 1; var v = new AnamnesisVersion { TenantId = tenantId, AnamnesisId = a.Id, VersionNumber = n, Ciphertext = a.DraftCiphertext ?? string.Empty, Nonce = a.DraftNonce ?? string.Empty, Tag = a.DraftTag ?? string.Empty, CreatedBy = userId, Reason = reason }; await anamnesisVersions.Create(v, ct); a.CurrentVersionId = v.Id; await anamneses.Update(a, ct); return Result.Success<object>(v); }
    public async Task<Result<object>> GetEvolutionAsync(int sessionId, int tenantId, CancellationToken ct) { var e = await GetOrCreateEvolution(sessionId, tenantId, ct); return Result.Success<object>(e); }
    public async Task<Result> AutosaveEvolutionAsync(int sessionId, int tenantId, string? ciphertext, string? nonce, string? tag, CancellationToken ct) { var e = await GetOrCreateEvolution(sessionId, tenantId, ct); e.DraftCiphertext = ciphertext; e.DraftNonce = nonce; e.DraftTag = tag; e.DraftUpdatedAt = DateTime.UtcNow; await evolutions.Update(e, ct); return Result.Success(); }
    public async Task<Result<object>> PublishEvolutionVersionAsync(int sessionId, int tenantId, int userId, string? reason, CancellationToken ct) { var e = await GetOrCreateEvolution(sessionId, tenantId, ct); var n = await evolutionVersions.CountByEvolutionAsync(e.Id, ct) + 1; var v = new EvolutionVersion { TenantId = tenantId, EvolutionId = e.Id, VersionNumber = n, Ciphertext = e.DraftCiphertext ?? string.Empty, Nonce = e.DraftNonce ?? string.Empty, Tag = e.DraftTag ?? string.Empty, CreatedBy = userId, Reason = reason }; await evolutionVersions.Create(v, ct); e.CurrentVersionId = v.Id; await evolutions.Update(e, ct); return Result.Success<object>(v); }
    public async Task<Result<object>> GetEvolutionVersionsAsync(int sessionId, int tenantId, CancellationToken ct) { var e = await evolutions.GetBySessionAndTenantAsync(sessionId, tenantId, ct); if (e is null) return Result.Success<object>(Array.Empty<object>()); return Result.Success<object>(await evolutionVersions.ListByEvolutionOrderedDescAsync(e.Id, ct)); }
    public async Task<Result<object>> GetAuditLogAsync(int recordId, int tenantId, CancellationToken ct) => Result.Success<object>(await auditLogs.ListByResourceAndTenantAsync(recordId, tenantId, ct));
    private async Task<Anamnesis> GetOrCreateAnamnesis(int recordId, int tenantId, CancellationToken ct) { var a = await anamneses.GetByRecordAndTenantAsync(recordId, tenantId, ct); if (a is not null) return a; a = new Anamnesis { RecordId = recordId, TenantId = tenantId }; await anamneses.Create(a, ct); return a; }
    private async Task<Evolution> GetOrCreateEvolution(int sessionId, int tenantId, CancellationToken ct) { var e = await evolutions.GetBySessionAndTenantAsync(sessionId, tenantId, ct); if (e is not null) return e; var recordId = await evolutions.GetFirstRecordIdForTenantAsync(tenantId, ct); e = new Evolution { SessionId = sessionId, TenantId = tenantId, RecordId = recordId ?? 0 }; await evolutions.Create(e, ct); return e; }
    private static async Task<Result<object>> OkOrNotFound<T>(Task<T?> task) where T : class => await task is { } value ? Result.Success<object>(value) : Result.Failure<object>(Error.NotFound("Clinical record not found."));
}
