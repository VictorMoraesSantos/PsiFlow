using ClinicalRecords.Application.DTOs.MedicalRecord;
using ClinicalRecords.Domain.Aggregates;

namespace ClinicalRecords.Application.Mapping
{
    public static class MedicalRecordMapper
    {
        public static MedicalRecordDTO ToDTO(this MedicalRecord entity) => new(
            entity.Id,
            entity.TenantId,
            entity.PatientId,
            entity.Name,
            entity.Status,
            entity.CreatedAt,
            entity.UpdatedAt);

        public static MedicalRecord ToEntity(this CreateMedicalRecordDTO dto) => new()
        {
            TenantId = dto.TenantId,
            PatientId = dto.PatientId,
            Name = dto.Name,
            Status = dto.Status
        };
    }
}
