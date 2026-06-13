using BuildingBlocks.CQRS.Extensions;
using BuildingBlocks.Results;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Sessions.Application.Contracts;
using Sessions.Application.DTOs.Session;
using Sessions.Application.Features.Sessions.Commands.Create;
using Sessions.Application.Features.Sessions.Commands.Delete;
using Sessions.Application.Features.Sessions.Commands.Update;
using Sessions.Application.Features.Sessions.Queries.GetAll;
using Sessions.Application.Features.Sessions.Queries.GetById;
using Sessions.Application.Features.Workflow;
using Sessions.Application.Services;

namespace Sessions.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSessionsApplication(this IServiceCollection services)
        {
            services.AddMediatorService()
                .AddHandler<CreateSessionCommand, Result<CreateSessionResult>, CreateSessionCommandHandler>()
                .AddHandler<UpdateSessionCommand, Result<bool>, UpdateSessionCommandHandler>()
                .AddHandler<DeleteSessionCommand, Result<bool>, DeleteSessionCommandHandler>()
                .AddHandler<GetSessionsQuery, Result<IEnumerable<SessionDTO?>>, GetSessionsQueryHandler>()
                .AddHandler<GetSessionByIdQuery, Result<SessionDTO?>, GetSessionByIdQueryHandler>()
                .AddHandler<GetPatientSessionsQuery, Result<IReadOnlyCollection<SessionResult>>, GetPatientSessionsQueryHandler>()
                .AddHandler<ChangeSessionStatusCommand, Result<bool>, ChangeSessionStatusCommandHandler>()
                .AddHandler<MarkPaymentReceivedCommand, Result<PaymentResult>, MarkPaymentReceivedCommandHandler>()
                .AddHandler<GetSessionPaymentQuery, Result<PaymentResult?>, GetSessionPaymentQueryHandler>()
                .AddHandler<SendReceiptCommand, Result<ReceiptResult>, SendReceiptCommandHandler>();

            services.AddScoped<IValidator<CreateSessionCommand>, CreateSessionCommandValidator>();
            services.AddScoped<IValidator<UpdateSessionCommand>, UpdateSessionCommandValidator>();
            services.AddScoped<IValidator<ChangeSessionStatusCommand>, ChangeSessionStatusCommandValidator>();

            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<ISessionWorkflowService, SessionWorkflowService>();
            return services;
        }
    }
}
