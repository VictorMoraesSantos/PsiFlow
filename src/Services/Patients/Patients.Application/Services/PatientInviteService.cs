using BuildingBlocks.Results;
using Patients.Application.Contracts;
using Patients.Application.Features.Workflow;
using Patients.Domain.Entities;
using Patients.Domain.Events;
using Patients.Domain.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace Patients.Application.Services;

public sealed class PatientInviteService : IPatientInviteService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IPatientStatusHistoryRepository _statusHistoryRepository;
    private readonly IPatientInviteRepository _inviteRepository;
    private readonly IPatientSessionsProvider? _sessionsProvider;

    public PatientInviteService(
        IPatientRepository patientRepository,
        IPatientStatusHistoryRepository statusHistoryRepository,
        IPatientInviteRepository inviteRepository,
        IPatientSessionsProvider? sessionsProvider = null)
    {
        _patientRepository = patientRepository;
        _statusHistoryRepository = statusHistoryRepository;
        _inviteRepository = inviteRepository;
        _sessionsProvider = sessionsProvider;
    }

    public async Task<Result> DeactivateAsync(int patientId, string? reason, int tenantId, int userId, string? correlationId, CancellationToken cancellationToken)
    {
        var patient = await _patientRepository.GetByIdAndTenantAsync(patientId, tenantId, cancellationToken);
        if (patient is null)
        {
            var existing = await _patientRepository.GetById(patientId, cancellationToken);
            return existing is null ? Result.Failure(Error.NotFound("Patient not found.")) : Result.Failure(Error.Forbidden("Patient belongs to another tenant."));
        }
        var from = patient.Status;
        patient.Status = PatientStatus.AdministrativeInactive;
        patient.DeactivatedAt = DateTime.UtcNow;
        patient.DeactivationReason = reason;
        patient.MarkAsUpdated();
        patient.AddDomainEvent(new PatientDeactivatedDomainEvent(patient.Id, patient.TenantId, from, patient.Status, reason, userId));
        await _statusHistoryRepository.Create(new PatientStatusHistory { TenantId = tenantId, PatientId = patientId, FromStatus = from, ToStatus = PatientStatus.AdministrativeInactive, Reason = reason, ChangedBy = userId, CorrelationId = correlationId }, cancellationToken);
        await _patientRepository.Update(patient, cancellationToken);
        return Result.Success();
    }

    public async Task<Result<object>> ChangeTreatmentStatusAsync(int patientId, string treatmentStatus, string? reason, int tenantId, int userId, string? correlationId, CancellationToken cancellationToken)
    {
        var patient = await _patientRepository.GetByIdAndTenantAsync(patientId, tenantId, cancellationToken);
        if (patient is null)
        {
            var existing = await _patientRepository.GetById(patientId, cancellationToken);
            return existing is null ? Result.Failure<object>(Error.NotFound("Patient not found.")) : Result.Failure<object>(Error.Forbidden("Patient belongs to another tenant."));
        }
        if (!PatientStatus.CanTransitionTreatment(patient.TreatmentStatus, treatmentStatus)) return Result.Failure<object>(Error.Failure("Invalid patient treatment status transition."));
        var from = patient.TreatmentStatus;
        patient.TreatmentStatus = treatmentStatus;
        patient.MarkAsUpdated();
        await _statusHistoryRepository.Create(new PatientStatusHistory { TenantId = tenantId, PatientId = patientId, FromStatus = from, ToStatus = treatmentStatus, Reason = reason, ChangedBy = userId, CorrelationId = correlationId }, cancellationToken);
        await _patientRepository.Update(patient, cancellationToken);
        return Result.Success<object>(patient);
    }

    public async Task<Result<object>> GetSessionsSummaryAsync(int patientId, int tenantId, CancellationToken cancellationToken)
    {
        if (_sessionsProvider is null) return Result.Success<object>(new { patientId, totalSessions = 0, completedSessions = 0, noShows = 0 });
        var sessionsResult = await _sessionsProvider.GetPatientSessionsAsync(patientId, tenantId, cancellationToken);
        if (!sessionsResult.IsSuccess) return Result.Failure<object>(sessionsResult.Error!);
        var sessions = sessionsResult.Value ?? Array.Empty<PatientSessionHistoryDTO>();
        return Result.Success<object>(new { patientId, totalSessions = sessions.Count, completedSessions = sessions.Count(x => x.Status == "completed"), noShows = sessions.Count(x => x.Status == "no_show") });
    }

    public async Task<Result<object>> CreateInviteAsync(string email, string? phone, int? patientId, int tenantId, int userId, CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim();
        var hasPending = await _inviteRepository.HasPendingForEmailAsync(tenantId, normalizedEmail, cancellationToken);
        if (hasPending) return Result.Failure<object>(Error.Conflict("Pending invite already exists for this email."));
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)).Replace("+", "-").Replace("/", "_").TrimEnd('=');
        var invite = new PatientInvite { TenantId = tenantId, PatientId = patientId, Email = normalizedEmail, Phone = phone, TokenHash = Hash(token), ExpiresAt = DateTime.UtcNow.AddDays(7), CreatedBy = userId };
        await _inviteRepository.Create(invite, cancellationToken);
        invite.AddDomainEvent(new PatientInvitedDomainEvent(invite.Id, invite.TenantId, invite.PatientId, invite.Email, invite.Phone, token));
        return Result.Success<object>(new { invite.Id, token, invite.ExpiresAt });
    }

    public async Task<Result<object>> PreviewInviteAsync(string token, CancellationToken cancellationToken)
    {
        var invite = await _inviteRepository.GetByTokenHashAsync(Hash(token), cancellationToken);
        return invite is null
            ? Result.Failure<object>(Error.NotFound("Invite not found."))
            : Result.Success<object>(new { invite.Email, invite.Status, invite.ExpiresAt, isExpired = invite.ExpiresAt <= DateTime.UtcNow });
    }

    public async Task<Result<object>> AcceptInviteAsync(string token, int userId, string userEmail, string? ipAddress, string? userAgent, CancellationToken cancellationToken)
    {
        var invite = await _inviteRepository.GetByTokenHashAsync(Hash(token), cancellationToken);
        if (invite is null) return Result.Failure<object>(Error.NotFound("Invite not found."));
        if (invite.Status == "accepted") return Result.Failure<object>(Error.Conflict("Invite already accepted."));
        if (invite.Status == "revoked") return Result.Failure<object>(Error.Failure("Invite revoked."));
        if (invite.ExpiresAt <= DateTime.UtcNow) return Result.Failure<object>(Error.Failure("Invite expired."));
        if (!string.Equals(invite.Email, userEmail.Trim(), StringComparison.OrdinalIgnoreCase)) return Result.Failure<object>(Error.Forbidden("Invite does not belong to authenticated user."));
        invite.Status = "accepted";
        invite.AcceptedAt = DateTime.UtcNow;
        invite.AcceptedByUserId = userId;
        invite.AcceptedByIp = ipAddress;
        invite.AcceptedByUserAgent = userAgent;
        if (invite.PatientId is int patientId)
        {
            var patient = await _patientRepository.GetByIdAndTenantAsync(patientId, invite.TenantId, cancellationToken);
            if (patient is not null) patient.UserId = userId;
        }
        await _inviteRepository.Update(invite, cancellationToken);
        invite.AddDomainEvent(new PatientInviteAcceptedDomainEvent(invite.Id, invite.TenantId, invite.PatientId, invite.Email, userId));
        return Result.Success<object>(new { invite.Id, invite.Status });
    }

    public async Task<Result> RevokeInviteAsync(int inviteId, int tenantId, CancellationToken cancellationToken)
    {
        var invite = await _inviteRepository.GetByIdAndTenantAsync(inviteId, tenantId, cancellationToken);
        if (invite is null)
        {
            var existing = await _inviteRepository.GetById(inviteId, cancellationToken);
            return existing is null ? Result.Failure(Error.NotFound("Invite not found.")) : Result.Failure(Error.Forbidden("Invite belongs to another tenant."));
        }
        invite.Status = "revoked";
        invite.RevokedAt = DateTime.UtcNow;
        return Result.Success();
    }

    private static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}
