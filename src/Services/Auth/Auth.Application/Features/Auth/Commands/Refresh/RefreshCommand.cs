using Auth.Application.DTOs.Auth;
using BuildingBlocks.CQRS.Requests.Commands;

namespace Auth.Application.Features.Auth.Commands.Refresh;

public sealed record RefreshCommand(string RefreshToken) : ICommand<TokenResponse>;
