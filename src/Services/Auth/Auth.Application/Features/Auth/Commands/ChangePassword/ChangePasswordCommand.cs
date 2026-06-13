using Auth.Application.DTOs.Auth;
using BuildingBlocks.CQRS.Requests.Commands;

namespace Auth.Application.Features.Auth.Commands.ChangePassword;

public sealed record ChangePasswordCommand(int UserId, ChangePasswordDTO Password) : ICommand;
