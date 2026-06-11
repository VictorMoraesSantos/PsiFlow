using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Sessions.Application.Contracts;

namespace Sessions.Application.Features.Workflow;

public sealed class SendReceiptCommandHandler(ISessionWorkflowService service) : ICommandHandler<SendReceiptCommand, ReceiptResult>
{
    public Task<Result<ReceiptResult>> Handle(SendReceiptCommand command, CancellationToken cancellationToken) =>
        service.SendReceiptAsync(command.SessionId, command.TenantId, cancellationToken);
}
