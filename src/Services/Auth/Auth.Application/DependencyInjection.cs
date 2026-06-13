using Auth.Application.Contracts;
using Auth.Application.DTOs.Auth;
using Auth.Application.Features.Auth.Commands.ChangePassword;
using Auth.Application.Features.Auth.Commands.ForgotPassword;
using Auth.Application.Features.Auth.Commands.Login;
using Auth.Application.Features.Auth.Commands.Logout;
using Auth.Application.Features.Auth.Commands.RecordConsent;
using Auth.Application.Features.Auth.Commands.Refresh;
using Auth.Application.Features.Auth.Commands.Register;
using Auth.Application.Features.Auth.Commands.ResetPassword;
using Auth.Application.Features.Auth.Commands.SetupMfa;
using Auth.Application.Features.Auth.Commands.VerifyMfa;
using Auth.Application.Features.Auth.Queries.Me;
using Auth.Application.Services;
using BuildingBlocks.CQRS.Extensions;
using BuildingBlocks.Results;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAuthApplication(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserService, UserService>();
            services.AddSingleton<EncryptionService>();
            services.AddSingleton<MfaLoginStore>();

            services.AddMediatorService()
                .AddHandler<RegisterCommand, Result<RegisterResult>, RegisterCommandHandler>()
                .AddHandler<LoginCommand, Result<object>, LoginCommandHandler>()
                .AddHandler<RefreshCommand, Result<TokenResponse>, RefreshCommandHandler>()
                .AddHandler<LogoutCommand, Result, LogoutCommandHandler>()
                .AddHandler<MeQuery, Result<MeResponse>, MeQueryHandler>()
                .AddHandler<RecordConsentCommand, Result, RecordConsentCommandHandler>()
                .AddHandler<ChangePasswordCommand, Result, ChangePasswordCommandHandler>()
                .AddHandler<ForgotPasswordCommand, Result, ForgotPasswordCommandHandler>()
                .AddHandler<ResetPasswordCommand, Result, ResetPasswordCommandHandler>()
                .AddHandler<SetupMfaCommand, Result<MfaSetupResult>, SetupMfaCommandHandler>()
                .AddHandler<VerifyMfaCommand, Result, VerifyMfaCommandHandler>();

            services.AddScoped<IValidator<RegisterCommand>, RegisterCommandValidator>();
            services.AddScoped<IValidator<LoginCommand>, LoginCommandValidator>();
            services.AddScoped<IValidator<RecordConsentCommand>, RecordConsentCommandValidator>();
            services.AddScoped<IValidator<ChangePasswordCommand>, ChangePasswordCommandValidator>();
            services.AddScoped<IValidator<ForgotPasswordCommand>, ForgotPasswordCommandValidator>();
            services.AddScoped<IValidator<ResetPasswordCommand>, ResetPasswordCommandValidator>();
            services.AddScoped<IValidator<VerifyMfaCommand>, VerifyMfaCommandValidator>();

            return services;
        }
    }
}
