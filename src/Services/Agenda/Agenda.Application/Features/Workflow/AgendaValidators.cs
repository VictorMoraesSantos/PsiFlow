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

public sealed class UpdateWeeklyAvailabilityCommandValidator : AbstractValidator<UpdateWeeklyAvailabilityCommand>
{
    private static readonly int[] AllowedSlotDurations = [30, 45, 50, 60];

    public UpdateWeeklyAvailabilityCommandValidator()
    {
        RuleFor(command => command.AvailabilityId).GreaterThan(0);
        RuleFor(command => command.Request.Weekday).InclusiveBetween(0, 6);
        RuleFor(command => command.Request.SlotDurationMinutes).Must(duration => AllowedSlotDurations.Contains(duration));
        RuleFor(command => command.Request.EndTime).GreaterThan(command => command.Request.StartTime);
    }
}

public sealed class CreateScheduleBlockCommandValidator : AbstractValidator<CreateScheduleBlockCommand>
{
    public CreateScheduleBlockCommandValidator()
    {
        RuleFor(command => command.Request.EndsAt).GreaterThan(command => command.Request.StartsAt);
    }
}

public sealed class GetAvailableSlotsQueryValidator : AbstractValidator<GetAvailableSlotsQuery>
{
    public GetAvailableSlotsQueryValidator()
    {
        RuleFor(query => query.Request.To).GreaterThan(query => query.Request.From);
    }
}
