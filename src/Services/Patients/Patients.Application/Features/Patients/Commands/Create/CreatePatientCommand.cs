using BuildingBlocks.CQRS.Requests.Commands;
using Patients.Application.DTOs.Patient;

namespace Patients.Application.Features.Patients.Commands.Create;

public sealed record CreatePatientCommand(CreatePatientDTO Patient, int RequestingUserId) : ICommand<CreatePatientResult>;
public sealed record CreatePatientResult(int Id);
