using ClinicalRecords.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PsiFlow.ClinicalRecords.Infrastructure.Persistence.Configuration;

public sealed class MedicalRecordConfiguration : IEntityTypeConfiguration<MedicalRecord>
{
    public void Configure(EntityTypeBuilder<MedicalRecord> builder)
    {
        builder.ToTable("medical_records");
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.TenantId).IsRequired();
        builder.Property(entity => entity.Name).HasMaxLength(160).IsRequired();
        builder.Property(entity => entity.Status).HasMaxLength(32).IsRequired();
        builder.Property(entity => entity.CreatedAt).IsRequired();
        builder.Property(entity => entity.UpdatedAt).IsRequired();
        builder.HasIndex(entity => new { entity.TenantId, entity.PatientId }).IsUnique();
    }
}
