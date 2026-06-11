using BuildingBlocks.CQRS.Requests.Commands;

namespace Notifications.Application.Features.Workflow;

public sealed record SendTestEmailCommand(string RecipientEmail, string TemplateKey, int? TenantId) : ICommand<object>;
