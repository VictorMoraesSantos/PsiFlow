using Auth.Application.Contracts;
using Auth.Application.DTOs.Auth;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;

namespace Auth.Application.Features.Auth.Commands.Refresh;

public sealed class RefreshCommandHandler : ICommandHandler<RefreshCommand, TokenResponse>
{
    private readonly IAuthService _service;

    public RefreshCommandHandler(IAuthService service)
    {
        _service = service;
    }

    public Task<Result<TokenResponse>> Handle(RefreshCommand command, CancellationToken cancellationToken) =>
        _service.RefreshAsync(command.RefreshToken, cancellationToken);
}
