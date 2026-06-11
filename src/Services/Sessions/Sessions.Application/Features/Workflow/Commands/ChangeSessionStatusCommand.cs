using BuildingBlocks.CQRS.Requests.Commands;
using Sessions.Application.Contracts;

namespace Sessions.Application.Features.Workflow;

public sealed record ChangeSessionStatusCommand(int SessionId, ChangeSessionStatusRequest Request, int TenantId, int UserId) : ICommand<bool>;
