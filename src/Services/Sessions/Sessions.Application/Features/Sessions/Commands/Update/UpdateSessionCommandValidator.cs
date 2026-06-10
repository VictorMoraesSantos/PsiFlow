using FluentValidation;

namespace Sessions.Application.Features.Sessions.Commands.Update;

public sealed class UpdateSessionCommandValidator : AbstractValidator<UpdateSessionCommand>
{
    public UpdateSessionCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
        RuleFor(command => command.Session.TenantId).GreaterThan(0);
        RuleFor(command => command.Session.Name).NotEmpty().MaximumLength(200);
        RuleFor(command => command.Session.AppointmentId).GreaterThan(0);
        RuleFor(command => command.Session.PatientId).GreaterThan(0);
        RuleFor(command => command.Session.PsychologistId).GreaterThan(0);
        RuleFor(command => command.Session.EndsAt).GreaterThan(command => command.Session.StartsAt);
    }
}
