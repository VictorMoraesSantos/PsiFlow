using Patients.Application.DTOs.Patient;
using Patients.Domain.Entities;

namespace Patients.Application.Mapping
{
    public static class PatientMapper
    {
        public static PatientDTO ToDTO(this Patient entity) => new(
            entity.Id,
            entity.TenantId,
            entity.FullName,
            entity.Email,
            entity.Phone,
            entity.BirthDate,
            entity.Status,
            entity.TreatmentStatus,
            entity.EmergencyContactName,
            entity.EmergencyContactPhone,
            entity.CreatedAt,
            entity.UpdatedAt,
            entity.DeactivatedAt,
            entity.DeactivationReason,
            entity.UserId);

        public static Patient ToEntity(this CreatePatientDTO dto) => new()
        {
            TenantId = dto.TenantId,
            FullName = dto.FullName,
            Email = dto.Email,
            Phone = dto.Phone,
            BirthDate = dto.BirthDate,
            EmergencyContactName = dto.EmergencyContactName,
            EmergencyContactPhone = dto.EmergencyContactPhone,
            Status = dto.Status,
            TreatmentStatus = dto.TreatmentStatus
        };
    }
}
