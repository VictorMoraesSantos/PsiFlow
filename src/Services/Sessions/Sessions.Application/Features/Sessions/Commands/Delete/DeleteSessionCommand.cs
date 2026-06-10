using BuildingBlocks.CQRS.Requests.Commands;

namespace Sessions.Application.Features.Sessions.Commands.Delete;

public sealed record DeleteSessionCommand(int Id) : ICommand<bool>;
