using FluentValidation;

namespace Agenda.Application.Features.Workflow;

public sealed class CreateWeeklyAvailabilityCommandValidator : AbstractValidator<CreateWeeklyAvailabilityCommand>
{
    private static readonly int[] AllowedSlotDurations = [30, 45, 50, 60];

    public CreateWeeklyAvailabilityCommandValidator()
    {
        RuleFor(command => command.Request.Weekday).InclusiveBetween(0, 6);
        RuleFor(command => command.Request.SlotDurationMinutes).Must(duration => AllowedSlotDurations.Contains(duration));
        RuleFor(command => command.Request.EndTime).GreaterThan(command => command.Request.StartTime);
    }
}
