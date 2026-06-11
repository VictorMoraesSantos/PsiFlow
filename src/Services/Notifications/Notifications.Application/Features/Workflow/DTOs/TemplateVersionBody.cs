namespace Notifications.Application.Features.Workflow;

public sealed record TemplateVersionBody(string Subject, string? BodyHtml, string? BodyText);
