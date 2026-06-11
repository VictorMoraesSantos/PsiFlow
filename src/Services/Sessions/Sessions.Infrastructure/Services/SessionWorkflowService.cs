using BuildingBlocks.Results;
using Microsoft.EntityFrameworkCore;
using PsiFlow.Sessions.Infrastructure.Persistence.Data;
using Sessions.Application.Contracts;
using Sessions.Domain.Entities;

namespace Sessions.Infrastructure.Services;

public sealed class SessionWorkflowService(SessionsDbContext dbContext) : ISessionWorkflowService
{
    public async Task<Result<IReadOnlyCollection<SessionResult>>> GetPatientSessionsAsync(int patientId, int tenantId, CancellationToken cancellationToken)
    {
        var sessions = await dbContext.Sessions
            .Where(session => session.PatientId == patientId && session.TenantId == tenantId)
            .OrderBy(session => session.StartsAt)
            .Select(session => new SessionResult(session.Id, session.AppointmentId, session.PatientId, session.PsychologistId, session.StartsAt, session.EndsAt, session.Status, session.Modality))
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyCollection<SessionResult>>(sessions);
    }

    public async Task<Result<bool>> ChangeStatusAsync(int sessionId, ChangeSessionStatusRequest request, int tenantId, int userId, CancellationToken cancellationToken)
    {
        var session = await dbContext.Sessions.FirstOrDefaultAsync(item => item.Id == sessionId && item.TenantId == tenantId, cancellationToken);
        if (session is null) return Result.Failure<bool>(Error.NotFound("Session not found"));

        var previousStatus = session.Status;
        session.Status = request.Status;
        dbContext.SessionStatusHistories.Add(new SessionStatusHistory
        {
            TenantId = tenantId,
            SessionId = session.Id,
            FromStatus = previousStatus,
            ToStatus = request.Status,
            ChangedBy = userId,
            Reason = request.Reason
        });
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(true);
    }

    public async Task<Result<PaymentResult>> MarkPaymentReceivedAsync(int sessionId, MarkPaymentReceivedRequest request, int tenantId, int userId, CancellationToken cancellationToken)
    {
        var sessionExists = await dbContext.Sessions.AnyAsync(item => item.Id == sessionId && item.TenantId == tenantId, cancellationToken);
        if (!sessionExists) return Result.Failure<PaymentResult>(Error.NotFound("Session not found"));

        var payment = await dbContext.ManualPayments.FirstOrDefaultAsync(item => item.SessionId == sessionId && item.TenantId == tenantId, cancellationToken);
        if (payment is null)
        {
            payment = new ManualPayment { TenantId = tenantId, SessionId = sessionId };
            dbContext.ManualPayments.Add(payment);
        }

        payment.Status = "received";
        payment.AmountCents = request.AmountCents ?? payment.AmountCents;
        payment.Currency = request.Currency ?? payment.Currency;
        payment.Notes = request.Notes ?? payment.Notes;
        payment.ReceivedAt = DateTime.UtcNow;
        payment.MarkedBy = userId;
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(ToResult(payment));
    }

    public async Task<Result<PaymentResult?>> GetPaymentAsync(int sessionId, int tenantId, CancellationToken cancellationToken)
    {
        var payment = await dbContext.ManualPayments.FirstOrDefaultAsync(item => item.SessionId == sessionId && item.TenantId == tenantId, cancellationToken);
        return Result.Success(payment is null ? null : ToResult(payment));
    }

    public async Task<Result<ReceiptResult>> SendReceiptAsync(int sessionId, int tenantId, CancellationToken cancellationToken)
    {
        var payment = await dbContext.ManualPayments.FirstOrDefaultAsync(item => item.SessionId == sessionId && item.TenantId == tenantId, cancellationToken);
        if (payment is null) return Result.Failure<ReceiptResult>(Error.NotFound("Payment not found"));

        var receipt = await dbContext.Receipts.FirstOrDefaultAsync(item => item.SessionId == sessionId && item.TenantId == tenantId && item.PaymentId == payment.Id, cancellationToken);
        if (receipt is null)
        {
            receipt = new Receipt { TenantId = tenantId, SessionId = sessionId, PaymentId = payment.Id };
            dbContext.Receipts.Add(receipt);
        }

        receipt.Status = "sent";
        receipt.SentAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(new ReceiptResult(receipt.Id, receipt.SessionId, receipt.PaymentId, receipt.Status, receipt.SentAt, receipt.NotificationId));
    }

    private static PaymentResult ToResult(ManualPayment payment) =>
        new(payment.Id, payment.SessionId, payment.Status, payment.AmountCents, payment.Currency, payment.ReceivedAt, payment.MarkedBy, payment.Notes);
}
