using BuildingBlocks.CQRS.Requests.Commands;
using ClinicalRecords.Application.DTOs.MedicalRecord;

namespace ClinicalRecords.Application.Features.MedicalRecords.Commands.Create;

public sealed record CreateMedicalRecordCommand(CreateMedicalRecordDTO MedicalRecord) : ICommand<CreateMedicalRecordResult>;
public sealed record CreateMedicalRecordResult(int Id);
