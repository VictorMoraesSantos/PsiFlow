namespace Sessions.Contracts.Events;

public sealed record SessionStatusChangedIntegrationEvent(Guid EventId, DateTime OccurredAtUtc, int SessionId, int TenantId, string FromStatus, string ToStatus, int ChangedBy, string? Reason);
public sealed record SessionStartedIntegrationEvent(Guid EventId, DateTime OccurredAtUtc, int SessionId, int TenantId, int PatientId, int PsychologistId);
public sealed record SessionCompletedIntegrationEvent(Guid EventId, DateTime OccurredAtUtc, int SessionId, int TenantId, int PatientId, int PsychologistId);
public sealed record SessionNoShowIntegrationEvent(Guid EventId, DateTime OccurredAtUtc, int SessionId, int TenantId, int PatientId, int PsychologistId);
public sealed record SessionCanceledIntegrationEvent(Guid EventId, DateTime OccurredAtUtc, int SessionId, int TenantId, int PatientId, int PsychologistId, string? Reason);
public sealed record PaymentMarkedReceivedIntegrationEvent(Guid EventId, DateTime OccurredAtUtc, int PaymentId, int SessionId, int TenantId, int? AmountCents, string Currency, int MarkedBy);
public sealed record ReceiptRequestedIntegrationEvent(Guid EventId, DateTime OccurredAtUtc, int ReceiptId, int SessionId, int PaymentId, int TenantId);
