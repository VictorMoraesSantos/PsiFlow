using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Patients.Application.Contracts;
using Patients.Application.DTOs.Patient;

namespace Patients.Application.Features.Patients.Queries.GetById;

public sealed class GetPatientByIdQueryHandler : IQueryHandler<GetPatientByIdQuery, PatientDTO?>
{
    private readonly IPatientService _service;

    public GetPatientByIdQueryHandler(IPatientService service)
    {
        _service = service;
    }

    public Task<Result<PatientDTO?>> Handle(GetPatientByIdQuery query, CancellationToken cancellationToken) =>
        _service.GetByIdAndTenantAsync(query.Id, query.TenantId, cancellationToken);
}
