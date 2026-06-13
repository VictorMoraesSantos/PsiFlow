using BuildingBlocks.Results;
using Sessions.Application.Contracts;
using Sessions.Domain.Entities;
using Sessions.Domain.Repositories;

namespace Sessions.Application.Services;

public sealed class SessionWorkflowService(
    ISessionRepository sessionRepository,
    ISessionStatusHistoryRepository sessionStatusHistoryRepository,
    IManualPaymentRepository manualPaymentRepository,
    IReceiptRepository receiptRepository) : ISessionWorkflowService
{
    public async Task<Result<IReadOnlyCollection<SessionResult>>> GetPatientSessionsAsync(int patientId, int tenantId, CancellationToken cancellationToken)
    {
        var sessions = await sessionRepository.ListByPatientOrderedAsync(patientId, tenantId, cancellationToken);
        var result = sessions
            .Select(session => new SessionResult(session.Id, session.AppointmentId, session.PatientId, session.PsychologistId, session.StartsAt, session.EndsAt, session.Status, session.Modality))
            .ToList();
        return Result.Success<IReadOnlyCollection<SessionResult>>(result);
    }

    public async Task<Result<bool>> ChangeStatusAsync(int sessionId, ChangeSessionStatusRequest request, int tenantId, int userId, CancellationToken cancellationToken)
    {
        var session = await sessionRepository.GetByIdAndTenantAsync(sessionId, tenantId, cancellationToken);
        if (session is null) return Result.Failure<bool>(Error.NotFound("Session not found"));

        var previousStatus = session.Status;
        session.Status = request.Status;
        await sessionStatusHistoryRepository.Create(new SessionStatusHistory
        {
            TenantId = tenantId,
            SessionId = session.Id,
            FromStatus = previousStatus,
            ToStatus = request.Status,
            ChangedBy = userId,
            Reason = request.Reason
        }, cancellationToken);
        await sessionStatusHistoryRepository.SaveChangesAsync(cancellationToken);
        return Result.Success(true);
    }

    public async Task<Result<PaymentResult>> MarkPaymentReceivedAsync(int sessionId, MarkPaymentReceivedRequest request, int tenantId, int userId, CancellationToken cancellationToken)
    {
        var session = await sessionRepository.GetByIdAndTenantAsync(sessionId, tenantId, cancellationToken);
        if (session is null) return Result.Failure<PaymentResult>(Error.NotFound("Session not found"));

        var payment = await manualPaymentRepository.GetBySessionAndTenantAsync(sessionId, tenantId, cancellationToken);
        if (payment is null)
        {
            payment = new ManualPayment { TenantId = tenantId, SessionId = sessionId };
            await manualPaymentRepository.Create(payment, cancellationToken);
        }

        payment.Status = "received";
        payment.AmountCents = request.AmountCents ?? payment.AmountCents;
        payment.Currency = request.Currency ?? payment.Currency;
        payment.Notes = request.Notes ?? payment.Notes;
        payment.ReceivedAt = DateTime.UtcNow;
        payment.MarkedBy = userId;
        await manualPaymentRepository.SaveChangesAsync(cancellationToken);
        return Result.Success(ToResult(payment));
    }

    public async Task<Result<PaymentResult?>> GetPaymentAsync(int sessionId, int tenantId, CancellationToken cancellationToken)
    {
        var payment = await manualPaymentRepository.GetBySessionAndTenantAsync(sessionId, tenantId, cancellationToken);
        return Result.Success(payment is null ? null : ToResult(payment));
    }

    public async Task<Result<ReceiptResult>> SendReceiptAsync(int sessionId, int tenantId, CancellationToken cancellationToken)
    {
        var payment = await manualPaymentRepository.GetBySessionAndTenantAsync(sessionId, tenantId, cancellationToken);
        if (payment is null) return Result.Failure<ReceiptResult>(Error.NotFound("Payment not found"));

        var receipt = await receiptRepository.GetBySessionPaymentAsync(sessionId, payment.Id, tenantId, cancellationToken);
        if (receipt is null)
        {
            receipt = new Receipt { TenantId = tenantId, SessionId = sessionId, PaymentId = payment.Id };
            await receiptRepository.Create(receipt, cancellationToken);
        }

        receipt.Status = "sent";
        receipt.SentAt = DateTime.UtcNow;
        await receiptRepository.SaveChangesAsync(cancellationToken);
        return Result.Success(new ReceiptResult(receipt.Id, receipt.SessionId, receipt.PaymentId, receipt.Status, receipt.SentAt, receipt.NotificationId));
    }

    private static PaymentResult ToResult(ManualPayment payment) =>
        new(payment.Id, payment.SessionId, payment.Status, payment.AmountCents, payment.Currency, payment.ReceivedAt, payment.MarkedBy, payment.Notes);
}
