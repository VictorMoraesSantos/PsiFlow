using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using ClinicalRecords.Application.Contracts;

namespace ClinicalRecords.Application.Features.MedicalRecords.Commands.Delete;

public sealed class DeleteMedicalRecordCommandHandler(IMedicalRecordService service) : ICommandHandler<DeleteMedicalRecordCommand, bool>
{
    public Task<Result<bool>> Handle(DeleteMedicalRecordCommand command, CancellationToken cancellationToken) =>
        service.DeleteAsync(command.Id, cancellationToken);
}
