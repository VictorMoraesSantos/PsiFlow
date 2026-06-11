using BuildingBlocks.CQRS.Requests.Commands;

namespace Patients.Application.Features.Workflow;

public sealed record AcceptPatientInviteCommand(string Token, int UserId) : ICommand<object>;
