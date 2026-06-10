using BuildingBlocks.CQRS.Requests.Commands;

namespace ClinicalRecords.Application.Features.MedicalRecords.Commands.Delete;

public sealed record DeleteMedicalRecordCommand(int Id) : ICommand<bool>;
