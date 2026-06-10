using FluentValidation;

namespace Notifications.Application.Features.NotificationTemplates.Commands.Create;

public sealed class CreateNotificationTemplateCommandValidator : AbstractValidator<CreateNotificationTemplateCommand>
{
    public CreateNotificationTemplateCommandValidator()
    {
        RuleFor(command => command.NotificationTemplate.Key).NotEmpty().MaximumLength(160);
        RuleFor(command => command.NotificationTemplate.Channel).NotEmpty().MaximumLength(80);
        RuleFor(command => command.NotificationTemplate.Name).NotEmpty().MaximumLength(200);
    }
}
