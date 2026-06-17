using BuildingBlocks.CQRS.Requests.Commands;

namespace Auth.Application.Features.Auth.Commands.VerifyEmail;

public sealed record VerifyEmailCommand(string Email, string Token) : ICommand;
