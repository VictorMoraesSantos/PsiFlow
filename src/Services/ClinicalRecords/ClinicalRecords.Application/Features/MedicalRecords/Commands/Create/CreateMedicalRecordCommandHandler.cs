using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using ClinicalRecords.Application.Contracts;
using FluentValidation;

namespace ClinicalRecords.Application.Features.MedicalRecords.Commands.Create;

public sealed class CreateMedicalRecordCommandHandler : ICommandHandler<CreateMedicalRecordCommand, CreateMedicalRecordResult>
{
    private readonly IMedicalRecordService _service;
    private readonly IValidator<CreateMedicalRecordCommand> _validator;

    public CreateMedicalRecordCommandHandler(IMedicalRecordService service, IValidator<CreateMedicalRecordCommand> validator)
    {
        _service = service;
        _validator = validator;
    }

    public async Task<Result<CreateMedicalRecordResult>> Handle(CreateMedicalRecordCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            return Result.Failure<CreateMedicalRecordResult>(Error.Failure(string.Join("; ", validation.Errors.Select(error => error.ErrorMessage))));

        var result = await _service.CreateAsync(command.MedicalRecord, cancellationToken);
        return result.IsSuccess
            ? Result.Success(new CreateMedicalRecordResult(result.Value))
            : Result.Failure<CreateMedicalRecordResult>(result.Error!);
    }
}
