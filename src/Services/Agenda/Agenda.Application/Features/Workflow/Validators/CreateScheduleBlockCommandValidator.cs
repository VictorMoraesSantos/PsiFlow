using FluentValidation;

namespace Agenda.Application.Features.Workflow;

public sealed class CreateScheduleBlockCommandValidator : AbstractValidator<CreateScheduleBlockCommand>
{
    public CreateScheduleBlockCommandValidator() => RuleFor(command => command.Request.EndsAt).GreaterThan(command => command.Request.StartsAt);
}
