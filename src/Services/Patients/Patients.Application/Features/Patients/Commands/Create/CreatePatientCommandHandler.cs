using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using FluentValidation;
using Patients.Application.Contracts;

namespace Patients.Application.Features.Patients.Commands.Create;

public sealed class CreatePatientCommandHandler : ICommandHandler<CreatePatientCommand, CreatePatientResult>
{
    private readonly IPatientService _service;
    private readonly IValidator<CreatePatientCommand> _validator;

    public CreatePatientCommandHandler(IPatientService service, IValidator<CreatePatientCommand> validator)
    {
        _service = service;
        _validator = validator;
    }

    public async Task<Result<CreatePatientResult>> Handle(CreatePatientCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            return Result.Failure<CreatePatientResult>(Error.Failure(string.Join("; ", validation.Errors.Select(error => error.ErrorMessage))));

        var result = await _service.CreateAsync(command.Patient, cancellationToken);
        return result.IsSuccess
            ? Result.Success(new CreatePatientResult(result.Value))
            : Result.Failure<CreatePatientResult>(result.Error!);
    }
}
