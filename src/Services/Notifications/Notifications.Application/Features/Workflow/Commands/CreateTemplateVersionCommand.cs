using BuildingBlocks.CQRS.Requests.Commands;

namespace Notifications.Application.Features.Workflow;

public sealed record CreateTemplateVersionCommand(int TemplateId, string Subject, string? BodyHtml, string? BodyText, int UserId) : ICommand<object>;
