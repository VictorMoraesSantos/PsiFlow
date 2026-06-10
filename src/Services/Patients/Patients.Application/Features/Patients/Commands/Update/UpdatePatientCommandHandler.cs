using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using FluentValidation;
using Patients.Application.Contracts;

namespace Patients.Application.Features.Patients.Commands.Update;

public sealed class UpdatePatientCommandHandler : ICommandHandler<UpdatePatientCommand, bool>
{
    private readonly IPatientService _service;
    private readonly IValidator<UpdatePatientCommand> _validator;

    public UpdatePatientCommandHandler(IPatientService service, IValidator<UpdatePatientCommand> validator)
    {
        _service = service;
        _validator = validator;
    }

    public async Task<Result<bool>> Handle(UpdatePatientCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            return Result.Failure<bool>(Error.Failure(string.Join("; ", validation.Errors.Select(error => error.ErrorMessage))));

        return await _service.UpdateAsync(command.Patient with { Id = command.Id }, cancellationToken);
    }
}
