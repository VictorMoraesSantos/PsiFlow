using BuildingBlocks.CQRS.Extensions;
using BuildingBlocks.Results;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Notifications.Application.DTOs.NotificationTemplate;
using Notifications.Application.Features.NotificationTemplates.Commands.Create;
using Notifications.Application.Features.NotificationTemplates.Commands.Delete;
using Notifications.Application.Features.NotificationTemplates.Commands.Update;
using Notifications.Application.Features.NotificationTemplates.Queries.GetAll;
using Notifications.Application.Features.NotificationTemplates.Queries.GetById;
using Notifications.Application.Features.Workflow;
using Notifications.Application.Services;

namespace Notifications.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddNotificationsApplication(this IServiceCollection services)
        {
            services.AddScoped<INotificationTemplateService, NotificationTemplateService>();
            services.AddScoped<INotificationWorkflowService, NotificationWorkflowService>();

            services.AddMediatorService()
                .AddHandler<CreateNotificationTemplateCommand, Result<CreateNotificationTemplateResult>, CreateNotificationTemplateCommandHandler>()
                .AddHandler<UpdateNotificationTemplateCommand, Result<bool>, UpdateNotificationTemplateCommandHandler>()
                .AddHandler<DeleteNotificationTemplateCommand, Result<bool>, DeleteNotificationTemplateCommandHandler>()
                .AddHandler<GetNotificationTemplatesQuery, Result<IEnumerable<NotificationTemplateDTO?>>, GetNotificationTemplatesQueryHandler>()
                .AddHandler<GetNotificationTemplateByIdQuery, Result<NotificationTemplateDTO?>, GetNotificationTemplateByIdQueryHandler>()
                .AddHandler<CreateTemplateVersionCommand, Result<object>, CreateTemplateVersionCommandHandler>()
                .AddHandler<GetNotificationLogsQuery, Result<object>, GetNotificationLogsQueryHandler>()
                .AddHandler<GetNotificationLogQuery, Result<object>, GetNotificationLogQueryHandler>()
                .AddHandler<SendTestEmailCommand, Result<object>, SendTestEmailCommandHandler>()
                .AddHandler<RetryNotificationCommand, Result, RetryNotificationCommandHandler>()
                .AddHandler<ScheduleReminderCommand, Result<object>, ScheduleReminderCommandHandler>();

            services.AddScoped<IValidator<CreateNotificationTemplateCommand>, CreateNotificationTemplateCommandValidator>();
            services.AddScoped<IValidator<UpdateNotificationTemplateCommand>, UpdateNotificationTemplateCommandValidator>();
            services.AddScoped<IValidator<CreateTemplateVersionCommand>, CreateTemplateVersionCommandValidator>();
            services.AddScoped<IValidator<SendTestEmailCommand>, SendTestEmailCommandValidator>();
            return services;
        }
    }
}
