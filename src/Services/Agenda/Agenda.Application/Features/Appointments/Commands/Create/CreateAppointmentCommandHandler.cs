using Agenda.Application.Contracts;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using FluentValidation;

namespace Agenda.Application.Features.Appointments.Commands.Create;

public sealed class CreateAppointmentCommandHandler : ICommandHandler<CreateAppointmentCommand, CreateAppointmentResult>
{
    private readonly IAppointmentService _service;
    private readonly IValidator<CreateAppointmentCommand> _validator;

    public CreateAppointmentCommandHandler(IAppointmentService service, IValidator<CreateAppointmentCommand> validator)
    {
        _service = service;
        _validator = validator;
    }

    public async Task<Result<CreateAppointmentResult>> Handle(CreateAppointmentCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            return Result.Failure<CreateAppointmentResult>(Error.Failure(string.Join("; ", validation.Errors.Select(error => error.ErrorMessage))));

        var result = await _service.CreateAsync(command.Appointment, cancellationToken);
        return result.IsSuccess
            ? Result.Success(new CreateAppointmentResult(result.Value))
            : Result.Failure<CreateAppointmentResult>(result.Error!);
    }
}
