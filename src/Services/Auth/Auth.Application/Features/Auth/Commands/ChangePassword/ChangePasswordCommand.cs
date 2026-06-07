using BuildingBlocks.CQRS.Requests.Commands;
using System.Security.Claims;

namespace Auth.Application.Features.Auth.Commands.ChangePassword
{
    public record ChangePasswordCommand(ClaimsPrincipal User, string CurrentPassword, string NewPassword) : ICommand;
}