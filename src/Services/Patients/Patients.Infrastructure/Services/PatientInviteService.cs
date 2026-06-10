using System.Security.Cryptography;
using System.Text;
using BuildingBlocks.Results;
using Microsoft.EntityFrameworkCore;
using Patients.Application.Contracts;
using Patients.Domain.Aggregates;
using PsiFlow.Patients.Infrastructure.Persistence;

namespace Patients.Infrastructure.Services;

public sealed class PatientInviteService(PatientsDbContext db) : IPatientInviteService
{
    public async Task<Result> DeactivateAsync(int patientId, string? reason, int tenantId, int userId, CancellationToken cancellationToken)
    {
        var patient = await db.Patients.FirstOrDefaultAsync(x => x.Id == patientId && x.TenantId == tenantId, cancellationToken);
        if (patient is null) return Result.Failure(Error.NotFound("Patient not found."));
        var from = patient.Status;
        patient.Status = "inactive";
        patient.DeactivatedAt = DateTime.UtcNow;
        patient.DeactivationReason = reason;
        patient.MarkAsUpdated();
        await db.PatientStatusHistories.AddAsync(new PatientStatusHistory { TenantId = tenantId, PatientId = patientId, FromStatus = from, ToStatus = "inactive", Reason = reason, ChangedBy = userId }, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<object>> ChangeTreatmentStatusAsync(int patientId, string treatmentStatus, string? reason, int tenantId, int userId, CancellationToken cancellationToken)
    {
        var patient = await db.Patients.FirstOrDefaultAsync(x => x.Id == patientId && x.TenantId == tenantId, cancellationToken);
        if (patient is null) return Result.Failure<object>(Error.NotFound("Patient not found."));
        var from = patient.TreatmentStatus;
        patient.TreatmentStatus = treatmentStatus;
        patient.MarkAsUpdated();
        await db.PatientStatusHistories.AddAsync(new PatientStatusHistory { TenantId = tenantId, PatientId = patientId, FromStatus = from, ToStatus = treatmentStatus, Reason = reason, ChangedBy = userId }, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        return Result.Success<object>(patient);
    }

    public Task<Result<object>> GetSessionsSummaryAsync(int patientId, CancellationToken cancellationToken) =>
        Task.FromResult(Result.Success<object>(new { patientId, totalSessions = 0, completedSessions = 0, noShows = 0 }));

    public async Task<Result<object>> CreateInviteAsync(string email, string? phone, int? patientId, int tenantId, int userId, CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim();
        var hasPending = await db.PatientInvites.AnyAsync(x => x.TenantId == tenantId && x.Email == normalizedEmail && x.Status == "pending" && x.ExpiresAt > DateTime.UtcNow, cancellationToken);
        if (hasPending) return Result.Failure<object>(Error.Conflict("Pending invite already exists for this email."));
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)).Replace("+", "-").Replace("/", "_").TrimEnd('=');
        var invite = new PatientInvite { TenantId = tenantId, PatientId = patientId, Email = normalizedEmail, Phone = phone, TokenHash = Hash(token), ExpiresAt = DateTime.UtcNow.AddDays(7), CreatedBy = userId };
        await db.PatientInvites.AddAsync(invite, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        return Result.Success<object>(new { invite.Id, token, invite.ExpiresAt });
    }

    public async Task<Result<object>> PreviewInviteAsync(string token, CancellationToken cancellationToken)
    {
        var invite = await db.PatientInvites.AsNoTracking().FirstOrDefaultAsync(x => x.TokenHash == Hash(token), cancellationToken);
        return invite is null
            ? Result.Failure<object>(Error.NotFound("Invite not found."))
            : Result.Success<object>(new { invite.Email, invite.Status, invite.ExpiresAt, isExpired = invite.ExpiresAt <= DateTime.UtcNow });
    }

    public async Task<Result<object>> AcceptInviteAsync(string token, int userId, CancellationToken cancellationToken)
    {
        var invite = await db.PatientInvites.FirstOrDefaultAsync(x => x.TokenHash == Hash(token), cancellationToken);
        if (invite is null) return Result.Failure<object>(Error.NotFound("Invite not found."));
        if (invite.Status != "pending" || invite.ExpiresAt <= DateTime.UtcNow) return Result.Failure<object>(Error.Failure("Invite is not active."));
        invite.Status = "accepted";
        invite.AcceptedAt = DateTime.UtcNow;
        if (invite.PatientId is int patientId)
        {
            var patient = await db.Patients.FirstOrDefaultAsync(x => x.Id == patientId && x.TenantId == invite.TenantId, cancellationToken);
            if (patient is not null) patient.UserId = userId;
        }
        await db.SaveChangesAsync(cancellationToken);
        return Result.Success<object>(new { invite.Id, invite.Status });
    }

    public async Task<Result> RevokeInviteAsync(int inviteId, int tenantId, CancellationToken cancellationToken)
    {
        var invite = await db.PatientInvites.FirstOrDefaultAsync(x => x.Id == inviteId && x.TenantId == tenantId, cancellationToken);
        if (invite is null) return Result.Failure(Error.NotFound("Invite not found."));
        invite.Status = "revoked";
        invite.RevokedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}
