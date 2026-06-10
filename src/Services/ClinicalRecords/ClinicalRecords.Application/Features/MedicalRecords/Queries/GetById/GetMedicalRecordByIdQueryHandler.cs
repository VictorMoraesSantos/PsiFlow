using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using ClinicalRecords.Application.Contracts;
using ClinicalRecords.Application.DTOs.MedicalRecord;

namespace ClinicalRecords.Application.Features.MedicalRecords.Queries.GetById;

public sealed class GetMedicalRecordByIdQueryHandler(IMedicalRecordService service) : IQueryHandler<GetMedicalRecordByIdQuery, MedicalRecordDTO?>
{
    public Task<Result<MedicalRecordDTO?>> Handle(GetMedicalRecordByIdQuery query, CancellationToken cancellationToken) =>
        service.GetByIdAsync(query.Id, cancellationToken);
}
