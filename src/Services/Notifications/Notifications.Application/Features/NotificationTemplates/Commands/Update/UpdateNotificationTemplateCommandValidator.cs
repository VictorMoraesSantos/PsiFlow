using FluentValidation;

namespace Notifications.Application.Features.NotificationTemplates.Commands.Update;

public sealed class UpdateNotificationTemplateCommandValidator : AbstractValidator<UpdateNotificationTemplateCommand>
{
    public UpdateNotificationTemplateCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
        RuleFor(command => command.NotificationTemplate.Key).NotEmpty().MaximumLength(160);
        RuleFor(command => command.NotificationTemplate.Channel).NotEmpty().MaximumLength(80);
        RuleFor(command => command.NotificationTemplate.Name).NotEmpty().MaximumLength(200);
    }
}
