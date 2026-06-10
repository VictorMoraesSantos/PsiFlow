using BuildingBlocks.CQRS.Requests.Queries;
using Patients.Application.DTOs.Patient;

namespace Patients.Application.Features.Patients.Queries.GetAll;

public sealed record GetPatientsQuery : IQuery<IEnumerable<PatientDTO?>>;
