using BuildingBlocks.CQRS.Requests.Commands;
using Sessions.Application.Contracts;

namespace Sessions.Application.Features.Workflow;

public sealed record MarkPaymentReceivedCommand(int SessionId, MarkPaymentReceivedRequest Request, int TenantId, int UserId) : ICommand<PaymentResult>;
