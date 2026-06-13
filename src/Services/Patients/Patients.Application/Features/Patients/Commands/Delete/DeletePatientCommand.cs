using BuildingBlocks.CQRS.Requests.Commands;

namespace Patients.Application.Features.Patients.Commands.Delete;

public sealed record DeletePatientCommand(int Id, int TenantId, int UserId) : ICommand<bool>;
