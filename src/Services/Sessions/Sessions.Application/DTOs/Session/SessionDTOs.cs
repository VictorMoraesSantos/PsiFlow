using Core.Application.DTO;
using Core.Application.DTOs;

namespace Sessions.Application.DTOs.Session
{
    public sealed record SessionDTO(
        int Id,
        int TenantId,
        string Name,
        int AppointmentId,
        int PatientId,
        int PsychologistId,
        DateTime StartsAt,
        DateTime EndsAt,
        string Status,
        string Modality,
        DateTime CreatedAt,
        DateTime? UpdatedAt) : DTOBase<int>(Id, CreatedAt, UpdatedAt);

    public sealed record CreateSessionDTO(
        int TenantId,
        string Name,
        int AppointmentId,
        int PatientId,
        int PsychologistId,
        DateTime StartsAt,
        DateTime EndsAt,
        string Status = "scheduled",
        string Modality = "online");

    public sealed record UpdateSessionDTO(
        int Id,
        int TenantId,
        string Name,
        int AppointmentId,
        int PatientId,
        int PsychologistId,
        DateTime StartsAt,
        DateTime EndsAt,
        string Status,
        string Modality);

    public sealed record SessionFilterDTO(
        int? TenantId,
        string? Search,
        int? Page = 1,
        int? PageSize = 20) : DomainQueryFilterDto(Page, PageSize);
}
