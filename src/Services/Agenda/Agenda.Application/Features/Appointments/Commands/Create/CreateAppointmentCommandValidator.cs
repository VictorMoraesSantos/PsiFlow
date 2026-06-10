using FluentValidation;

namespace Agenda.Application.Features.Appointments.Commands.Create;

public sealed class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
{
    public CreateAppointmentCommandValidator()
    {
        RuleFor(command => command.Appointment.TenantId).GreaterThan(0);
        RuleFor(command => command.Appointment.Name).NotEmpty().MaximumLength(200);
        RuleFor(command => command.Appointment.PatientId).GreaterThan(0);
        RuleFor(command => command.Appointment.PsychologistId).GreaterThan(0);
        RuleFor(command => command.Appointment.EndsAt).GreaterThan(command => command.Appointment.StartsAt);
    }
}
