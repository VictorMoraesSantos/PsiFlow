using Microsoft.EntityFrameworkCore;
using Agenda.Domain.Entities;

namespace PsiFlow.Agenda.Infrastructure.Persistence.Data;

public sealed class AgendaDbContext : DbContext
{
    public AgendaDbContext(DbContextOptions<AgendaDbContext> options) : base(options) { }

    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Availability> Availabilities => Set<Availability>();
    public DbSet<ScheduleBlock> ScheduleBlocks => Set<ScheduleBlock>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("agenda");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AgendaDbContext).Assembly);
        modelBuilder.Entity<Availability>().ToTable("availability").HasKey(entity => entity.Id);
        modelBuilder.Entity<ScheduleBlock>().ToTable("schedule_blocks").HasKey(entity => entity.Id);
    }
}
