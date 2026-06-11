using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Sessions.Application.Contracts;

namespace Sessions.Application.Features.Workflow;

public sealed class MarkPaymentReceivedCommandHandler(ISessionWorkflowService service) : ICommandHandler<MarkPaymentReceivedCommand, PaymentResult>
{
    public Task<Result<PaymentResult>> Handle(MarkPaymentReceivedCommand command, CancellationToken cancellationToken) =>
        service.MarkPaymentReceivedAsync(command.SessionId, command.Request, command.TenantId, command.UserId, cancellationToken);
}
