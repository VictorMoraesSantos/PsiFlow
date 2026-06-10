using BuildingBlocks.CQRS.Requests.Commands;
using BuildingBlocks.CQRS.Requests.Queries;
using Sessions.Application.Contracts;

namespace Sessions.Application.Features.Workflow;

public sealed record GetPatientSessionsQuery(int PatientId, int TenantId) : IQuery<IReadOnlyCollection<SessionResult>>;
public sealed record ChangeSessionStatusCommand(int SessionId, ChangeSessionStatusRequest Request, int TenantId, int UserId) : ICommand<bool>;
public sealed record MarkPaymentReceivedCommand(int SessionId, MarkPaymentReceivedRequest Request, int TenantId, int UserId) : ICommand<PaymentResult>;
public sealed record GetSessionPaymentQuery(int SessionId, int TenantId) : IQuery<PaymentResult?>;
public sealed record SendReceiptCommand(int SessionId, int TenantId) : ICommand<ReceiptResult>;
