using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Patients.Application.Contracts;

namespace Patients.Application.Features.Patients.Commands.Delete;

public sealed class DeletePatientCommandHandler : ICommandHandler<DeletePatientCommand, bool>
{
    private readonly IPatientInviteService _service;

    public DeletePatientCommandHandler(IPatientInviteService service)
    {
        _service = service;
    }

    public async Task<Result<bool>> Handle(DeletePatientCommand command, CancellationToken cancellationToken)
    {
        var result = await _service.DeactivateAsync(command.Id, "Soft delete requested", command.TenantId, command.UserId, null, cancellationToken);
        return result.IsSuccess ? Result.Success(true) : Result.Failure<bool>(result.Error!);
    }
}
