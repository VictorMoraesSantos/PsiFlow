using Core.Application.DTOs;

namespace Agenda.Application.DTOs.Appointment
{
    public sealed record AppointmentDTO(
        int Id,
        int TenantId,
        string Name,
        int PatientId,
        int PsychologistId,
        DateTime StartsAt,
        DateTime EndsAt,
        string Modality,
        string Status,
        bool LateCancel,
        DateTime? CanceledAt,
        int? CanceledBy,
        string? CancelReason,
        int CreatedBy,
        DateTime CreatedAt,
        DateTime? UpdatedAt) : DTOBase<int>(Id, CreatedAt, UpdatedAt);

    public sealed record CreateAppointmentDTO(
        int TenantId,
        string Name,
        int PatientId,
        int PsychologistId,
        DateTime StartsAt,
        DateTime EndsAt,
        string Modality = "online",
        string Status = "scheduled",
        int CreatedBy = default);

    public sealed record UpdateAppointmentDTO(
        int Id,
        int TenantId,
        string Name,
        int PatientId,
        int PsychologistId,
        DateTime StartsAt,
        DateTime EndsAt,
        string Modality,
        string Status);

    public sealed record AppointmentFilterDTO(
        int? TenantId,
        string? Search,
        int? Page = 1,
        int? PageSize = 20) : DomainQueryFilterDto(Page, PageSize);
}
