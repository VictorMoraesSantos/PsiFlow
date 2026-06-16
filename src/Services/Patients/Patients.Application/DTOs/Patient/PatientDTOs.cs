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
        string? Address,
        string? DocumentNumber,
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
        string? Address,
        string? DocumentNumber,
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
        string? EmergencyContactPhone,
        string? Address,
        string? DocumentNumber);

    public sealed record PatientFilterDTO(
        int? TenantId,
        string? Search,
        int? Page = 1,
        int? PageSize = 20) : DomainQueryFilterDto(Page, PageSize);

    public sealed record PatientAdministrativeProfileDTO(
        PatientDTO Patient,
        IEnumerable<PatientAdministrativeNoteDTO> AdministrativeNotes,
        IEnumerable<PatientStatusTimelineItemDTO> Timeline,
        object SessionsSummary,
        IEnumerable<Patients.Application.Contracts.PatientSessionHistoryDTO> SessionsHistory);

    public sealed record PatchPatientAdministrativeDTO(
        string? FullName,
        string? Email,
        string? Phone,
        DateOnly? BirthDate,
        string? EmergencyContactName,
        string? EmergencyContactPhone,
        string? Address,
        string? DocumentNumber);

    public sealed record CreatePatientAdministrativeNoteDTO(string Text);

    public sealed record PatientAdministrativeNoteDTO(int Id, int PatientId, string Text, int CreatedBy, DateTime CreatedAt);

    public sealed record PatientStatusTimelineItemDTO(int Id, int PatientId, string FromStatus, string ToStatus, string? Reason, int ChangedBy, DateTime CreatedAt);

    public sealed record UpsertEmergencyContactDTO(string? Name, string? Phone, string? Relationship, string? Email);

    public sealed record UpsertResponsibleLegalDTO(string? Name, string? Document, string? Relationship, string? Phone, string? Email);

    public sealed record ResponsibleLegalDTO(int Id, int PatientId, string FullName, string? Email, string Phone, string Relationship);
}
