using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.CQRS.Requests.Commands;
using BuildingBlocks.CQRS.Requests.Queries;
using BuildingBlocks.Results;
using FluentValidation;
using Patients.Application.Contracts;

namespace Patients.Application.Features.Workflow;

public sealed record DeactivatePatientCommand(int PatientId, string? Reason, int TenantId, int UserId) : ICommand;
public sealed record ChangeTreatmentStatusCommand(int PatientId, string TreatmentStatus, string? Reason, int TenantId, int UserId) : ICommand<object>;
public sealed record GetPatientSessionsSummaryQuery(int PatientId) : IQuery<object>;
public sealed record CreatePatientInviteCommand(string Email, string? Phone, int? PatientId, int TenantId, int UserId) : ICommand<object>;
public sealed record PreviewPatientInviteQuery(string Token) : IQuery<object>;
public sealed record AcceptPatientInviteCommand(string Token, int UserId) : ICommand<object>;
public sealed record RevokePatientInviteCommand(int InviteId, int TenantId) : ICommand;

public sealed class CreatePatientInviteCommandValidator : AbstractValidator<CreatePatientInviteCommand>
{
    public CreatePatientInviteCommandValidator() => RuleFor(x => x.Email).NotEmpty().EmailAddress();
}

public sealed class ChangeTreatmentStatusCommandValidator : AbstractValidator<ChangeTreatmentStatusCommand>
{
    public ChangeTreatmentStatusCommandValidator() => RuleFor(x => x.TreatmentStatus).Must(x => PatientStatus.AllowedTreatmentStatuses.Contains(x));
}

public static class PatientStatus
{
    public static readonly HashSet<string> AllowedTreatmentStatuses = ["active_treatment", "discharged", "paused", "screening"];
}

public sealed class DeactivatePatientCommandHandler(IPatientInviteService service) : ICommandHandler<DeactivatePatientCommand>
{
    public Task<Result> Handle(DeactivatePatientCommand command, CancellationToken cancellationToken) =>
        service.DeactivateAsync(command.PatientId, command.Reason, command.TenantId, command.UserId, cancellationToken);
}

public sealed class ChangeTreatmentStatusCommandHandler(IPatientInviteService service, IValidator<ChangeTreatmentStatusCommand> validator) : ICommandHandler<ChangeTreatmentStatusCommand, object>
{
    public async Task<Result<object>> Handle(ChangeTreatmentStatusCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        return validation.IsValid
            ? await service.ChangeTreatmentStatusAsync(command.PatientId, command.TreatmentStatus, command.Reason, command.TenantId, command.UserId, cancellationToken)
            : Result.Failure<object>(Error.Failure(string.Join("; ", validation.Errors.Select(x => x.ErrorMessage))));
    }
}

public sealed class GetPatientSessionsSummaryQueryHandler(IPatientInviteService service) : IQueryHandler<GetPatientSessionsSummaryQuery, object>
{
    public Task<Result<object>> Handle(GetPatientSessionsSummaryQuery query, CancellationToken cancellationToken) => service.GetSessionsSummaryAsync(query.PatientId, cancellationToken);
}

public sealed class CreatePatientInviteCommandHandler(IPatientInviteService service, IValidator<CreatePatientInviteCommand> validator) : ICommandHandler<CreatePatientInviteCommand, object>
{
    public async Task<Result<object>> Handle(CreatePatientInviteCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        return validation.IsValid
            ? await service.CreateInviteAsync(command.Email, command.Phone, command.PatientId, command.TenantId, command.UserId, cancellationToken)
            : Result.Failure<object>(Error.Failure(string.Join("; ", validation.Errors.Select(x => x.ErrorMessage))));
    }
}

public sealed class PreviewPatientInviteQueryHandler(IPatientInviteService service) : IQueryHandler<PreviewPatientInviteQuery, object>
{
    public Task<Result<object>> Handle(PreviewPatientInviteQuery query, CancellationToken cancellationToken) => service.PreviewInviteAsync(query.Token, cancellationToken);
}

public sealed class AcceptPatientInviteCommandHandler(IPatientInviteService service) : ICommandHandler<AcceptPatientInviteCommand, object>
{
    public Task<Result<object>> Handle(AcceptPatientInviteCommand command, CancellationToken cancellationToken) => service.AcceptInviteAsync(command.Token, command.UserId, cancellationToken);
}

public sealed class RevokePatientInviteCommandHandler(IPatientInviteService service) : ICommandHandler<RevokePatientInviteCommand>
{
    public Task<Result> Handle(RevokePatientInviteCommand command, CancellationToken cancellationToken) => service.RevokeInviteAsync(command.InviteId, command.TenantId, cancellationToken);
}
