using Auth.Application.DTOs.Auth;
using BuildingBlocks.CQRS.Requests.Commands;

namespace Auth.Application.Features.Auth.Commands.Register;

public sealed record RegisterCommand(RegisterDTO Data) : ICommand<RegisterResult>;
