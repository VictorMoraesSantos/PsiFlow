using BuildingBlocks.CQRS.Requests.Commands;
using Sessions.Application.DTOs.Session;

namespace Sessions.Application.Features.Sessions.Commands.Create;

public sealed record CreateSessionCommand(CreateSessionDTO Session) : ICommand<CreateSessionResult>;
public sealed record CreateSessionResult(int Id);
