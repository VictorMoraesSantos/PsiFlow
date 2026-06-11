using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Sessions.Application.Contracts;

namespace Sessions.Application.Features.Workflow;

public sealed class GetSessionPaymentQueryHandler(ISessionWorkflowService service) : IQueryHandler<GetSessionPaymentQuery, PaymentResult?>
{
    public Task<Result<PaymentResult?>> Handle(GetSessionPaymentQuery query, CancellationToken cancellationToken) =>
        service.GetPaymentAsync(query.SessionId, query.TenantId, cancellationToken);
}
