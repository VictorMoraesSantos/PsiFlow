using Auth.Application.DTOs.Auth;
using BuildingBlocks.CQRS.Requests.Commands;

namespace Auth.Application.Features.Auth.Commands.ResetPassword;

public sealed record ResetPasswordCommand(ResetPasswordDTO Data) : ICommand;
