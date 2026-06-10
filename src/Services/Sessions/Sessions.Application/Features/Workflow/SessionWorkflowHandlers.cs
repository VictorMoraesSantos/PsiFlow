using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using FluentValidation;
using Sessions.Application.Contracts;

namespace Sessions.Application.Features.Workflow;

public sealed class GetPatientSessionsQueryHandler(ISessionWorkflowService service) : IQueryHandler<GetPatientSessionsQuery, IReadOnlyCollection<SessionResult>>
{
    public Task<Result<IReadOnlyCollection<SessionResult>>> Handle(GetPatientSessionsQuery query, CancellationToken cancellationToken) =>
        service.GetPatientSessionsAsync(query.PatientId, query.TenantId, cancellationToken);
}

public sealed class ChangeSessionStatusCommandHandler(ISessionWorkflowService service, IValidator<ChangeSessionStatusCommand> validator) : ICommandHandler<ChangeSessionStatusCommand, bool>
{
    public async Task<Result<bool>> Handle(ChangeSessionStatusCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        return validation.IsValid
            ? await service.ChangeStatusAsync(command.SessionId, command.Request, command.TenantId, command.UserId, cancellationToken)
            : Result.Failure<bool>(Error.Failure(string.Join("; ", validation.Errors.Select(error => error.ErrorMessage))));
    }
}

public sealed class MarkPaymentReceivedCommandHandler(ISessionWorkflowService service) : ICommandHandler<MarkPaymentReceivedCommand, PaymentResult>
{
    public Task<Result<PaymentResult>> Handle(MarkPaymentReceivedCommand command, CancellationToken cancellationToken) =>
        service.MarkPaymentReceivedAsync(command.SessionId, command.Request, command.TenantId, command.UserId, cancellationToken);
}

public sealed class GetSessionPaymentQueryHandler(ISessionWorkflowService service) : IQueryHandler<GetSessionPaymentQuery, PaymentResult?>
{
    public Task<Result<PaymentResult?>> Handle(GetSessionPaymentQuery query, CancellationToken cancellationToken) =>
        service.GetPaymentAsync(query.SessionId, query.TenantId, cancellationToken);
}

public sealed class SendReceiptCommandHandler(ISessionWorkflowService service) : ICommandHandler<SendReceiptCommand, ReceiptResult>
{
    public Task<Result<ReceiptResult>> Handle(SendReceiptCommand command, CancellationToken cancellationToken) =>
        service.SendReceiptAsync(command.SessionId, command.TenantId, cancellationToken);
}
