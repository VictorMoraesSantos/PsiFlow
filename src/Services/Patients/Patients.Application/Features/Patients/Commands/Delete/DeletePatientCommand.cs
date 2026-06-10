using BuildingBlocks.CQRS.Requests.Commands;

namespace Patients.Application.Features.Patients.Commands.Delete;

public sealed record DeletePatientCommand(int Id) : ICommand<bool>;
