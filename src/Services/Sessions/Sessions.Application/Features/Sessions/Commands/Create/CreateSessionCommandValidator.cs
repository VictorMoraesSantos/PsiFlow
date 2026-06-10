using FluentValidation;

namespace Sessions.Application.Features.Sessions.Commands.Create;

public sealed class CreateSessionCommandValidator : AbstractValidator<CreateSessionCommand>
{
    public CreateSessionCommandValidator()
    {
        RuleFor(command => command.Session.TenantId).GreaterThan(0);
        RuleFor(command => command.Session.Name).NotEmpty().MaximumLength(200);
        RuleFor(command => command.Session.AppointmentId).GreaterThan(0);
        RuleFor(command => command.Session.PatientId).GreaterThan(0);
        RuleFor(command => command.Session.PsychologistId).GreaterThan(0);
        RuleFor(command => command.Session.EndsAt).GreaterThan(command => command.Session.StartsAt);
    }
}
