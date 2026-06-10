using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using ClinicalRecords.Application.Contracts;
using FluentValidation;

namespace ClinicalRecords.Application.Features.MedicalRecords.Commands.Update;

public sealed class UpdateMedicalRecordCommandHandler : ICommandHandler<UpdateMedicalRecordCommand, bool>
{
    private readonly IMedicalRecordService _service;
    private readonly IValidator<UpdateMedicalRecordCommand> _validator;

    public UpdateMedicalRecordCommandHandler(IMedicalRecordService service, IValidator<UpdateMedicalRecordCommand> validator)
    {
        _service = service;
        _validator = validator;
    }

    public async Task<Result<bool>> Handle(UpdateMedicalRecordCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            return Result.Failure<bool>(Error.Failure(string.Join("; ", validation.Errors.Select(error => error.ErrorMessage))));

        return await _service.UpdateAsync(command.MedicalRecord with { Id = command.Id }, cancellationToken);
    }
}
