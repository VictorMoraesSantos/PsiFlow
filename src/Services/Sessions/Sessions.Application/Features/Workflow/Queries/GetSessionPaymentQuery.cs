using BuildingBlocks.CQRS.Requests.Queries;
using Sessions.Application.Contracts;

namespace Sessions.Application.Features.Workflow;

public sealed record GetSessionPaymentQuery(int SessionId, int TenantId) : IQuery<PaymentResult?>;
