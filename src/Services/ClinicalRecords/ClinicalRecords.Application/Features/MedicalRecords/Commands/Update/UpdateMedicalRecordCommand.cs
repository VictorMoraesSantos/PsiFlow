using BuildingBlocks.CQRS.Requests.Commands;
using ClinicalRecords.Application.DTOs.MedicalRecord;

namespace ClinicalRecords.Application.Features.MedicalRecords.Commands.Update;

public sealed record UpdateMedicalRecordCommand(int Id, UpdateMedicalRecordDTO MedicalRecord) : ICommand<bool>;
