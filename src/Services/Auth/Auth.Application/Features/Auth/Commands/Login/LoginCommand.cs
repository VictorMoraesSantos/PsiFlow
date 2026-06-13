using Auth.Application.DTOs.Auth;
using BuildingBlocks.CQRS.Requests.Commands;

namespace Auth.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(LoginDTO Credentials) : ICommand<TokenResponse>;
