using BuildingBlocks.CQRS.Requests.Commands;
using Sessions.Application.Contracts;

namespace Sessions.Application.Features.Workflow;

public sealed record SendReceiptCommand(int SessionId, int TenantId) : ICommand<ReceiptResult>;
