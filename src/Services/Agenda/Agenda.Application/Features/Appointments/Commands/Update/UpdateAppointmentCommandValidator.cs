using FluentValidation;

namespace Agenda.Application.Features.Appointments.Commands.Update;

public sealed class UpdateAppointmentCommandValidator : AbstractValidator<UpdateAppointmentCommand>
{
    public UpdateAppointmentCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
        RuleFor(command => command.Appointment.TenantId).GreaterThan(0);
        RuleFor(command => command.Appointment.Name).NotEmpty().MaximumLength(200);
        RuleFor(command => command.Appointment.PatientId).GreaterThan(0);
        RuleFor(command => command.Appointment.PsychologistId).GreaterThan(0);
        RuleFor(command => command.Appointment.EndsAt).GreaterThan(command => command.Appointment.StartsAt);
    }
}
