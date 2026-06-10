using Agenda.Application.DTOs.Appointment;
using Agenda.Domain.Aggregates;

namespace Agenda.Application.Mapping
{
    public static class AppointmentMapper
    {
        public static AppointmentDTO ToDTO(this Appointment entity) => new(
            entity.Id,
            entity.TenantId,
            entity.Name,
            entity.PatientId,
            entity.PsychologistId,
            entity.StartsAt,
            entity.EndsAt,
            entity.Modality,
            entity.Status,
            entity.LateCancel,
            entity.CanceledAt,
            entity.CanceledBy,
            entity.CancelReason,
            entity.CreatedBy,
            entity.CreatedAt,
            entity.UpdatedAt);

        public static Appointment ToEntity(this CreateAppointmentDTO dto) => new()
        {
            TenantId = dto.TenantId,
            Name = dto.Name,
            PatientId = dto.PatientId,
            PsychologistId = dto.PsychologistId,
            StartsAt = dto.StartsAt,
            EndsAt = dto.EndsAt,
            Modality = dto.Modality,
            Status = dto.Status,
            CreatedBy = dto.CreatedBy
        };
    }
}
