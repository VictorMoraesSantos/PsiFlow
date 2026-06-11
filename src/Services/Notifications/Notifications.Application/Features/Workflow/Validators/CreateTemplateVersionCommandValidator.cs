using FluentValidation;

namespace Notifications.Application.Features.Workflow;

public sealed class CreateTemplateVersionCommandValidator : AbstractValidator<CreateTemplateVersionCommand>
{
    public CreateTemplateVersionCommandValidator() => RuleFor(x => x.Subject).NotEmpty();
}
