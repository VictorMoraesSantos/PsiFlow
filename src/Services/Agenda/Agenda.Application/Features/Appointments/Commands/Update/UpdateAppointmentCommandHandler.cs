using Agenda.Application.Contracts;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using FluentValidation;

namespace Agenda.Application.Features.Appointments.Commands.Update;

public sealed class UpdateAppointmentCommandHandler : ICommandHandler<UpdateAppointmentCommand, bool>
{
    private readonly IAppointmentService _service;
    private readonly IValidator<UpdateAppointmentCommand> _validator;

    public UpdateAppointmentCommandHandler(IAppointmentService service, IValidator<UpdateAppointmentCommand> validator)
    {
        _service = service;
        _validator = validator;
    }

    public async Task<Result<bool>> Handle(UpdateAppointmentCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            return Result.Failure<bool>(Error.Failure(string.Join("; ", validation.Errors.Select(error => error.ErrorMessage))));

        return await _service.UpdateAsync(command.Appointment with { Id = command.Id }, cancellationToken);
    }
}
