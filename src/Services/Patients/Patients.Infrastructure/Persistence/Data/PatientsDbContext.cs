using Microsoft.EntityFrameworkCore;
using Patients.Domain.Entities;

namespace PsiFlow.Patients.Infrastructure.Persistence.Data;

public sealed class PatientsDbContext : DbContext
{
    public PatientsDbContext(DbContextOptions<PatientsDbContext> options) : base(options) { }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<PatientInvite> PatientInvites => Set<PatientInvite>();
    public DbSet<ResponsibleLegal> ResponsibleLegals => Set<ResponsibleLegal>();
    public DbSet<PatientAdministrativeNote> PatientAdministrativeNotes => Set<PatientAdministrativeNote>();
    public DbSet<PatientStatusHistory> PatientStatusHistories => Set<PatientStatusHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("patients");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PatientsDbContext).Assembly);
        modelBuilder.Entity<PatientInvite>().ToTable("patient_invites").HasKey(entity => entity.Id);
        modelBuilder.Entity<ResponsibleLegal>().ToTable("responsible_legals").HasKey(entity => entity.Id);
        modelBuilder.Entity<PatientAdministrativeNote>().ToTable("patient_administrative_notes").HasKey(entity => entity.Id);
        modelBuilder.Entity<PatientStatusHistory>().ToTable("patient_status_histories").HasKey(entity => entity.Id);
    }
}
