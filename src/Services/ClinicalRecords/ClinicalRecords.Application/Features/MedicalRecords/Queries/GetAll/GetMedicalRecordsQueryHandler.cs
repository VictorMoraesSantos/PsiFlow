using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using ClinicalRecords.Application.Contracts;
using ClinicalRecords.Application.DTOs.MedicalRecord;

namespace ClinicalRecords.Application.Features.MedicalRecords.Queries.GetAll;

public sealed class GetMedicalRecordsQueryHandler(IMedicalRecordService service) : IQueryHandler<GetMedicalRecordsQuery, IEnumerable<MedicalRecordDTO?>>
{
    public Task<Result<IEnumerable<MedicalRecordDTO?>>> Handle(GetMedicalRecordsQuery query, CancellationToken cancellationToken) =>
        service.GetAllAsync(cancellationToken);
}
