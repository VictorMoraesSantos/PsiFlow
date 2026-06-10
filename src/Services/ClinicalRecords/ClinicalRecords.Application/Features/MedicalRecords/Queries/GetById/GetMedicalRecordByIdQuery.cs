using BuildingBlocks.CQRS.Requests.Queries;
using ClinicalRecords.Application.DTOs.MedicalRecord;

namespace ClinicalRecords.Application.Features.MedicalRecords.Queries.GetById;

public sealed record GetMedicalRecordByIdQuery(int Id) : IQuery<MedicalRecordDTO?>;
