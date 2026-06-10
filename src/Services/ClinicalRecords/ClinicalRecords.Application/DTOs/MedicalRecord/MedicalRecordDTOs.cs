using Core.Application.DTO;
using Core.Application.DTOs;

namespace ClinicalRecords.Application.DTOs.MedicalRecord
{
    public sealed record MedicalRecordDTO(
        int Id,
        int TenantId,
        int PatientId,
        string Name,
        string Status,
        DateTime CreatedAt,
        DateTime? UpdatedAt) : DTOBase<int>(Id, CreatedAt, UpdatedAt);

    public sealed record CreateMedicalRecordDTO(
        int TenantId,
        int PatientId,
        string Name,
        string Status = "active");

    public sealed record UpdateMedicalRecordDTO(
        int Id,
        int TenantId,
        int PatientId,
        string Name,
        string Status);

    public sealed record MedicalRecordFilterDTO(
        int? TenantId,
        string? Search,
        int? Page = 1,
        int? PageSize = 20) : DomainQueryFilterDto(Page, PageSize);
}
