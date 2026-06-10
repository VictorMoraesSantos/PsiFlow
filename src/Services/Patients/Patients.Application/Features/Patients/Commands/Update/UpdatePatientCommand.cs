using BuildingBlocks.CQRS.Requests.Commands;
using Patients.Application.DTOs.Patient;

namespace Patients.Application.Features.Patients.Commands.Update;

public sealed record UpdatePatientCommand(int Id, UpdatePatientDTO Patient) : ICommand<bool>;
