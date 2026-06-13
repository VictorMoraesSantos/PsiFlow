using Auth.Application.DTOs.Auth;
using BuildingBlocks.CQRS.Requests.Commands;

namespace Auth.Application.Features.Auth.Commands.ForgotPassword;

public sealed record ForgotPasswordCommand(ForgotPasswordDTO Data) : ICommand;
