using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Patients.Application.Contracts;
using Patients.Application.DTOs.Patient;

namespace Patients.Application.Features.Patients.Queries.GetAll;

public sealed class GetPatientsQueryHandler : IQueryHandler<GetPatientsQuery, IEnumerable<PatientDTO?>>
{
    private readonly IPatientService _service;

    public GetPatientsQueryHandler(IPatientService service)
    {
        _service = service;
    }

    public Task<Result<IEnumerable<PatientDTO?>>> Handle(GetPatientsQuery query, CancellationToken cancellationToken) =>
        _service.ListByTenantAsync(query.TenantId, cancellationToken);
}
