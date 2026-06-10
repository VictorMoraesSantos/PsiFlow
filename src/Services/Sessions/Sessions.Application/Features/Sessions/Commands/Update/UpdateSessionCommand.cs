using BuildingBlocks.CQRS.Requests.Commands;
using Sessions.Application.DTOs.Session;

namespace Sessions.Application.Features.Sessions.Commands.Update;

public sealed record UpdateSessionCommand(int Id, UpdateSessionDTO Session) : ICommand<bool>;
