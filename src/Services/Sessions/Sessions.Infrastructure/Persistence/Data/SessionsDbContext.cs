using Microsoft.EntityFrameworkCore;
using Sessions.Domain.Entities;

namespace PsiFlow.Sessions.Infrastructure.Persistence.Data;

public sealed class SessionsDbContext : DbContext
{
    public SessionsDbContext(DbContextOptions<SessionsDbContext> options) : base(options) { }

    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<SessionStatusHistory> SessionStatusHistories => Set<SessionStatusHistory>();
    public DbSet<ManualPayment> ManualPayments => Set<ManualPayment>();
    public DbSet<Receipt> Receipts => Set<Receipt>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("sessions");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SessionsDbContext).Assembly);
        modelBuilder.Entity<SessionStatusHistory>().ToTable("session_status_histories").HasKey(entity => entity.Id);
        modelBuilder.Entity<ManualPayment>().ToTable("manual_payments").HasKey(entity => entity.Id);
        modelBuilder.Entity<Receipt>().ToTable("receipts").HasKey(entity => entity.Id);
    }
}
