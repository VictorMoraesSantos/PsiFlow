using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.CQRS.Requests.Commands;
using BuildingBlocks.CQRS.Requests.Queries;
using BuildingBlocks.Results;
using FluentValidation;
using Notifications.Application.Contracts;

namespace Notifications.Application.Features.Workflow;

public sealed record TemplateVersionBody(string Subject, string? BodyHtml, string? BodyText);
public sealed record TestEmailBody(string RecipientEmail, string TemplateKey);
public sealed record ScheduleReminderBody(string NotificationType, DateTime ScheduledFor, string? RecipientEmail, int? RecipientUserId, string? PayloadJson);
public sealed record CreateTemplateVersionCommand(int TemplateId, string Subject, string? BodyHtml, string? BodyText, int UserId) : ICommand<object>;
public sealed record GetNotificationLogsQuery : IQuery<object>;
public sealed record GetNotificationLogQuery(int NotificationId) : IQuery<object>;
public sealed record SendTestEmailCommand(string RecipientEmail, string TemplateKey, int? TenantId) : ICommand<object>;
public sealed record RetryNotificationCommand(int NotificationId) : ICommand;
public sealed record ScheduleReminderCommand(string NotificationType, DateTime ScheduledFor, string? RecipientEmail, int? RecipientUserId, int? TenantId, string? PayloadJson) : ICommand<object>;
public sealed class CreateTemplateVersionCommandValidator : AbstractValidator<CreateTemplateVersionCommand> { public CreateTemplateVersionCommandValidator() { RuleFor(x => x.Subject).NotEmpty(); } }
public sealed class SendTestEmailCommandValidator : AbstractValidator<SendTestEmailCommand> { public SendTestEmailCommandValidator() { RuleFor(x => x.RecipientEmail).NotEmpty().EmailAddress(); RuleFor(x => x.TemplateKey).NotEmpty(); } }
public sealed class CreateTemplateVersionCommandHandler(INotificationWorkflowService service, IValidator<CreateTemplateVersionCommand> validator) : ICommandHandler<CreateTemplateVersionCommand, object> { public async Task<Result<object>> Handle(CreateTemplateVersionCommand c, CancellationToken ct) { var v = await validator.ValidateAsync(c, ct); return v.IsValid ? await service.CreateTemplateVersionAsync(c.TemplateId, c.Subject, c.BodyHtml, c.BodyText, c.UserId, ct) : Result.Failure<object>(Error.Failure(string.Join("; ", v.Errors.Select(x => x.ErrorMessage)))); } }
public sealed class GetNotificationLogsQueryHandler(INotificationWorkflowService service) : IQueryHandler<GetNotificationLogsQuery, object> { public Task<Result<object>> Handle(GetNotificationLogsQuery q, CancellationToken ct) => service.GetLogsAsync(ct); }
public sealed class GetNotificationLogQueryHandler(INotificationWorkflowService service) : IQueryHandler<GetNotificationLogQuery, object> { public Task<Result<object>> Handle(GetNotificationLogQuery q, CancellationToken ct) => service.GetLogAsync(q.NotificationId, ct); }
public sealed class SendTestEmailCommandHandler(INotificationWorkflowService service, IValidator<SendTestEmailCommand> validator) : ICommandHandler<SendTestEmailCommand, object> { public async Task<Result<object>> Handle(SendTestEmailCommand c, CancellationToken ct) { var v = await validator.ValidateAsync(c, ct); return v.IsValid ? await service.SendTestEmailAsync(c.RecipientEmail, c.TemplateKey, c.TenantId, ct) : Result.Failure<object>(Error.Failure(string.Join("; ", v.Errors.Select(x => x.ErrorMessage)))); } }
public sealed class RetryNotificationCommandHandler(INotificationWorkflowService service) : ICommandHandler<RetryNotificationCommand> { public Task<Result> Handle(RetryNotificationCommand c, CancellationToken ct) => service.RetryAsync(c.NotificationId, ct); }
public sealed class ScheduleReminderCommandHandler(INotificationWorkflowService service) : ICommandHandler<ScheduleReminderCommand, object> { public Task<Result<object>> Handle(ScheduleReminderCommand c, CancellationToken ct) => service.ScheduleRemindersAsync(c.NotificationType, c.ScheduledFor, c.RecipientEmail, c.RecipientUserId, c.TenantId, c.PayloadJson, ct); }
