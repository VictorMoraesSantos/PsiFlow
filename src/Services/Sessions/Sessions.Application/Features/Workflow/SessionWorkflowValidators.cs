using FluentValidation;

namespace Sessions.Application.Features.Workflow;

public sealed class ChangeSessionStatusCommandValidator : AbstractValidator<ChangeSessionStatusCommand>
{
    private static readonly string[] AllowedStatuses = ["started", "completed", "no_show", "canceled"];

    public ChangeSessionStatusCommandValidator()
    {
        RuleFor(command => command.SessionId).GreaterThan(0);
        RuleFor(command => command.Request.Status).Must(status => AllowedStatuses.Contains(status)).WithMessage("Invalid session status");
    }
}
