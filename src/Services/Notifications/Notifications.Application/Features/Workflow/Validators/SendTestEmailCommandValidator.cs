using FluentValidation;

namespace Notifications.Application.Features.Workflow;

public sealed class SendTestEmailCommandValidator : AbstractValidator<SendTestEmailCommand>
{
    public SendTestEmailCommandValidator()
    {
        RuleFor(x => x.RecipientEmail).NotEmpty().EmailAddress();
        RuleFor(x => x.TemplateKey).NotEmpty();
    }
}
