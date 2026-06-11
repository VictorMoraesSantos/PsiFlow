using Agenda.Application.Contracts;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using FluentValidation;

namespace Agenda.Application.Features.Workflow;

public sealed class CreateWeeklyAvailabilityCommandHandler(IAgendaService service, IValidator<CreateWeeklyAvailabilityCommand> validator)
    : ICommandHandler<CreateWeeklyAvailabilityCommand, WeeklyAvailabilityResult>
{
    public async Task<Result<WeeklyAvailabilityResult>> Handle(CreateWeeklyAvailabilityCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        return validation.IsValid
            ? await service.CreateWeeklyAvailabilityAsync(command.Request, command.TenantId, cancellationToken)
            : Result.Failure<WeeklyAvailabilityResult>(Error.Failure(string.Join("; ", validation.Errors.Select(error => error.ErrorMessage))));
    }
}

public sealed class UpdateWeeklyAvailabilityCommandHandler(IAgendaService service, IValidator<UpdateWeeklyAvailabilityCommand> validator)
    : ICommandHandler<UpdateWeeklyAvailabilityCommand, bool>
{
    public async Task<Result<bool>> Handle(UpdateWeeklyAvailabilityCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        return validation.IsValid
            ? await service.UpdateWeeklyAvailabilityAsync(command.AvailabilityId, command.Request, command.TenantId, cancellationToken)
            : Result.Failure<bool>(Error.Failure(string.Join("; ", validation.Errors.Select(error => error.ErrorMessage))));
    }
}

public sealed class GetWeeklyAvailabilitiesQueryHandler(IAgendaService service)
    : IQueryHandler<GetWeeklyAvailabilitiesQuery, IReadOnlyCollection<WeeklyAvailabilityResult>>
{
    public Task<Result<IReadOnlyCollection<WeeklyAvailabilityResult>>> Handle(GetWeeklyAvailabilitiesQuery query, CancellationToken cancellationToken) =>
        service.GetWeeklyAvailabilitiesAsync(query.TenantId, cancellationToken);
}

public sealed class DeleteWeeklyAvailabilityCommandHandler(IAgendaService service) : ICommandHandler<DeleteWeeklyAvailabilityCommand, bool>
{
    public Task<Result<bool>> Handle(DeleteWeeklyAvailabilityCommand command, CancellationToken cancellationToken) =>
        service.DeleteWeeklyAvailabilityAsync(command.AvailabilityId, command.TenantId, cancellationToken);
}

public sealed class CreateScheduleBlockCommandHandler(IAgendaService service, IValidator<CreateScheduleBlockCommand> validator)
    : ICommandHandler<CreateScheduleBlockCommand, ScheduleBlockResult>
{
    public async Task<Result<ScheduleBlockResult>> Handle(CreateScheduleBlockCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        return validation.IsValid
            ? await service.CreateScheduleBlockAsync(command.Request, command.TenantId, command.UserId, cancellationToken)
            : Result.Failure<ScheduleBlockResult>(Error.Failure(string.Join("; ", validation.Errors.Select(error => error.ErrorMessage))));
    }
}

public sealed class DeleteScheduleBlockCommandHandler(IAgendaService service) : ICommandHandler<DeleteScheduleBlockCommand, bool>
{
    public Task<Result<bool>> Handle(DeleteScheduleBlockCommand command, CancellationToken cancellationToken) =>
        service.DeleteScheduleBlockAsync(command.BlockId, command.TenantId, cancellationToken);
}

public sealed class GetScheduleBlocksQueryHandler(IAgendaService service)
    : IQueryHandler<GetScheduleBlocksQuery, IReadOnlyCollection<ScheduleBlockResult>>
{
    public Task<Result<IReadOnlyCollection<ScheduleBlockResult>>> Handle(GetScheduleBlocksQuery query, CancellationToken cancellationToken) =>
        service.GetScheduleBlocksAsync(query.TenantId, cancellationToken);
}

public sealed class GetAvailableSlotsQueryHandler(IAgendaService service, IValidator<GetAvailableSlotsQuery> validator)
    : IQueryHandler<GetAvailableSlotsQuery, IReadOnlyCollection<AvailableSlotResult>>
{
    public async Task<Result<IReadOnlyCollection<AvailableSlotResult>>> Handle(GetAvailableSlotsQuery query, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(query, cancellationToken);
        return validation.IsValid
            ? await service.GetAvailableSlotsAsync(query.Request, query.TenantId, cancellationToken)
            : Result.Failure<IReadOnlyCollection<AvailableSlotResult>>(Error.Failure(string.Join("; ", validation.Errors.Select(error => error.ErrorMessage))));
    }
}

public sealed class CancelAppointmentCommandHandler(IAgendaService service) : ICommandHandler<CancelAppointmentCommand, bool>
{
    public Task<Result<bool>> Handle(CancelAppointmentCommand command, CancellationToken cancellationToken) =>
        service.CancelAppointmentAsync(command.AppointmentId, command.Request, command.TenantId, command.UserId, cancellationToken);
}
