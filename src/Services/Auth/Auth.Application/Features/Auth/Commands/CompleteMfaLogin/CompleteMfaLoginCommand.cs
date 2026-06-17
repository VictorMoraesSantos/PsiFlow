using Auth.Application.DTOs.Auth;
using BuildingBlocks.CQRS.Requests.Commands;

namespace Auth.Application.Features.Auth.Commands.CompleteMfaLogin;

public sealed record CompleteMfaLoginCommand(string MfaToken, string Code) : ICommand<TokenResponse>;
