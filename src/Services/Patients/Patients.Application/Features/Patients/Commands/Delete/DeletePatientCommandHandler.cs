using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Patients.Application.Contracts;

namespace Patients.Application.Features.Patients.Commands.Delete;

public sealed class DeletePatientCommandHandler : ICommandHandler<DeletePatientCommand, bool>
{
    private readonly IPatientService _service;

    public DeletePatientCommandHandler(IPatientService service)
    {
        _service = service;
    }

    public Task<Result<bool>> Handle(DeletePatientCommand command, CancellationToken cancellationToken) =>
        _service.DeleteAsync(command.Id, cancellationToken);
}
