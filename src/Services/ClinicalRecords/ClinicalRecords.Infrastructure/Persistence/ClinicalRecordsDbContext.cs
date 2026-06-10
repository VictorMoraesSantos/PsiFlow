using Microsoft.EntityFrameworkCore;
using ClinicalRecords.Domain.Aggregates;

namespace PsiFlow.ClinicalRecords.Infrastructure.Persistence;

public sealed class ClinicalRecordsDbContext : DbContext
{
    public ClinicalRecordsDbContext(DbContextOptions<ClinicalRecordsDbContext> options) : base(options) { }

    public DbSet<MedicalRecord> MedicalRecords => Set<MedicalRecord>();
    public DbSet<Anamnesis> Anamneses => Set<Anamnesis>();
    public DbSet<AnamnesisVersion> AnamnesisVersions => Set<AnamnesisVersion>();
    public DbSet<Evolution> Evolutions => Set<Evolution>();
    public DbSet<EvolutionVersion> EvolutionVersions => Set<EvolutionVersion>();
    public DbSet<ClinicalAuditLog> ClinicalAuditLogs => Set<ClinicalAuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("clinical_records");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClinicalRecordsDbContext).Assembly);
        modelBuilder.Entity<Anamnesis>().ToTable("anamneses").HasKey(entity => entity.Id);
        modelBuilder.Entity<AnamnesisVersion>().ToTable("anamnesis_versions").HasKey(entity => entity.Id);
        modelBuilder.Entity<Evolution>().ToTable("evolutions").HasKey(entity => entity.Id);
        modelBuilder.Entity<EvolutionVersion>().ToTable("evolution_versions").HasKey(entity => entity.Id);
        modelBuilder.Entity<ClinicalAuditLog>().ToTable("clinical_audit_logs").HasKey(entity => entity.Id);
    }
}
