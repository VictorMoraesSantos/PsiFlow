using Core.Application.DTO;
using Core.Application.DTOs;

namespace Patients.Application.DTOs.Patient
{
    public sealed record PatientDTO(
        int Id,
        int TenantId,
        string FullName,
        string Email,
        string Phone,
        DateOnly? BirthDate,
        string Status,
        string TreatmentStatus,
        string? EmergencyContactName,
        string? EmergencyContactPhone,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        DateTime? DeactivatedAt,
        string? DeactivationReason,
        int? UserId) : DTOBase<int>(Id, CreatedAt, UpdatedAt);

    public sealed record CreatePatientDTO(
        int TenantId,
        string FullName,
        string Email,
        string Phone,
        DateOnly? BirthDate,
        string? EmergencyContactName,
        string? EmergencyContactPhone,
        string Status = "active",
        string TreatmentStatus = "screening");

    public sealed record UpdatePatientDTO(
        int Id,
        int TenantId,
        string FullName,
        string Email,
        string Phone,
        DateOnly? BirthDate,
        string Status,
        string TreatmentStatus,
        string? EmergencyContactName,
        string? EmergencyContactPhone);

    public sealed record PatientFilterDTO(
        int? TenantId,
        string? Search,
        int? Page = 1,
        int? PageSize = 20) : DomainQueryFilterDto(Page, PageSize);
}
