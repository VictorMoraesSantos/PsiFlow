using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using FluentValidation;
using Patients.Application.Contracts;

namespace Patients.Application.Features.Workflow;

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
