using Auth.Application.DTOs.Auth;
using BuildingBlocks.CQRS.Requests.Commands;

namespace Auth.Application.Features.Auth.Commands.SetupMfa;

public sealed record SetupMfaCommand(int UserId) : ICommand<MfaSetupResult>;
