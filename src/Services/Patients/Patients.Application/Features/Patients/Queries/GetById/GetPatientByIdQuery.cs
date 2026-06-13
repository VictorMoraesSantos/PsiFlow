using BuildingBlocks.CQRS.Requests.Queries;
using Patients.Application.DTOs.Patient;

namespace Patients.Application.Features.Patients.Queries.GetById;

public sealed record GetPatientByIdQuery(int Id, int TenantId) : IQuery<PatientDTO?>;
