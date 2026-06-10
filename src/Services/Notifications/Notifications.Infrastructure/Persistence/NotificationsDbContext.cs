using Microsoft.EntityFrameworkCore;
using Notifications.Domain.Aggregates;

namespace PsiFlow.Notifications.Infrastructure.Persistence;

public sealed class NotificationsDbContext : DbContext
{
    public NotificationsDbContext(DbContextOptions<NotificationsDbContext> options) : base(options) { }

    public DbSet<NotificationTemplate> NotificationTemplates => Set<NotificationTemplate>();
    public DbSet<NotificationTemplateVersion> NotificationTemplateVersions => Set<NotificationTemplateVersion>();
    public DbSet<ScheduledNotification> ScheduledNotifications => Set<ScheduledNotification>();
    public DbSet<NotificationLog> NotificationLogs => Set<NotificationLog>();
    public DbSet<NotificationPreference> NotificationPreferences => Set<NotificationPreference>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("notifications");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NotificationsDbContext).Assembly);
        modelBuilder.Entity<NotificationTemplateVersion>().ToTable("notification_template_versions").HasKey(entity => entity.Id);
        modelBuilder.Entity<ScheduledNotification>().ToTable("scheduled_notifications").HasKey(entity => entity.Id);
        modelBuilder.Entity<NotificationLog>().ToTable("notification_logs").HasKey(entity => entity.Id);
        modelBuilder.Entity<NotificationPreference>().ToTable("notification_preferences").HasKey(entity => entity.Id);
    }
}
