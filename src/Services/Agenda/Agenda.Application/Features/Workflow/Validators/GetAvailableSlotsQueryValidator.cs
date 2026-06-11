using FluentValidation;

namespace Agenda.Application.Features.Workflow;

public sealed class GetAvailableSlotsQueryValidator : AbstractValidator<GetAvailableSlotsQuery>
{
    public GetAvailableSlotsQueryValidator() => RuleFor(query => query.Request.To).GreaterThan(query => query.Request.From);
}
