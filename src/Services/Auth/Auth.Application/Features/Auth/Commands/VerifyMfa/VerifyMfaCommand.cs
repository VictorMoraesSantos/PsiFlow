using Auth.Application.DTOs.Auth;
using BuildingBlocks.CQRS.Requests.Commands;

namespace Auth.Application.Features.Auth.Commands.VerifyMfa;

public sealed record VerifyMfaCommand(int UserId, MfaVerifyDTO Code) : ICommand;
