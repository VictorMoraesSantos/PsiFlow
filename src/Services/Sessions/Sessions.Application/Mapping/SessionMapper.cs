using Sessions.Application.DTOs.Session;
using Sessions.Domain.Aggregates;

namespace Sessions.Application.Mapping
{
    public static class SessionMapper
    {
        public static SessionDTO ToDTO(this Session entity) => new(
            entity.Id,
            entity.TenantId,
            entity.Name,
            entity.AppointmentId,
            entity.PatientId,
            entity.PsychologistId,
            entity.StartsAt,
            entity.EndsAt,
            entity.Status,
            entity.Modality,
            entity.CreatedAt,
            entity.UpdatedAt);

        public static Session ToEntity(this CreateSessionDTO dto) => new()
        {
            TenantId = dto.TenantId,
            Name = dto.Name,
            AppointmentId = dto.AppointmentId,
            PatientId = dto.PatientId,
            PsychologistId = dto.PsychologistId,
            StartsAt = dto.StartsAt,
            EndsAt = dto.EndsAt,
            Status = dto.Status,
            Modality = dto.Modality
        };
    }
}
