using BuildingBlocks.CQRS.Requests.Commands;

namespace Auth.Application.Features.Auth.Commands.RequestEmailVerification;

public sealed record RequestEmailVerificationCommand(string Email) : ICommand<string>;
