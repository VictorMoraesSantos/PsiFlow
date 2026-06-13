using BuildingBlocks.CQRS.Requests.Commands;

namespace Auth.Application.Features.Auth.Commands.Logout;

public sealed record LogoutCommand(int UserId) : ICommand;
