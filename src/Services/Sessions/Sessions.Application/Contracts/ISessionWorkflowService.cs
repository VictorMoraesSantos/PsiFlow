using BuildingBlocks.Results;

namespace Sessions.Application.Contracts;

public interface ISessionWorkflowService
{
    Task<Result<IReadOnlyCollection<SessionResult>>> GetPatientSessionsAsync(int patientId, int tenantId, CancellationToken cancellationToken);
    Task<Result<bool>> ChangeStatusAsync(int sessionId, ChangeSessionStatusRequest request, int tenantId, int userId, CancellationToken cancellationToken);
    Task<Result<PaymentResult>> MarkPaymentReceivedAsync(int sessionId, MarkPaymentReceivedRequest request, int tenantId, int userId, CancellationToken cancellationToken);
    Task<Result<PaymentResult?>> GetPaymentAsync(int sessionId, int tenantId, CancellationToken cancellationToken);
    Task<Result<ReceiptResult>> SendReceiptAsync(int sessionId, int tenantId, CancellationToken cancellationToken);
}

public sealed record SessionResult(int Id, int AppointmentId, int PatientId, int PsychologistId, DateTime StartsAt, DateTime EndsAt, string Status, string Modality, string PaymentStatus, string? OnlineSessionLink);
public sealed record ChangeSessionStatusRequest(string Status, string? Reason);
public sealed record MarkPaymentReceivedRequest(int? AmountCents, string? Currency, string? Notes);
public sealed record PaymentResult(int Id, int SessionId, string Status, int? AmountCents, string Currency, DateTime? ReceivedAt, int? MarkedBy, string? Notes);
public sealed record ReceiptResult(int Id, int SessionId, int PaymentId, string Status, DateTime? SentAt, int? NotificationId);
