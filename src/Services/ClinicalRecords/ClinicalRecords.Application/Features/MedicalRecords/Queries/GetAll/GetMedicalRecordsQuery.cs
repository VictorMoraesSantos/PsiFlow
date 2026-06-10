using BuildingBlocks.CQRS.Requests.Queries;
using ClinicalRecords.Application.DTOs.MedicalRecord;

namespace ClinicalRecords.Application.Features.MedicalRecords.Queries.GetAll;

public sealed record GetMedicalRecordsQuery : IQuery<IEnumerable<MedicalRecordDTO?>>;
