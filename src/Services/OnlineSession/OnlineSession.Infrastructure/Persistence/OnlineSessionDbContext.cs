using Microsoft.EntityFrameworkCore;
using OnlineSession.Domain.Aggregates;

namespace PsiFlow.OnlineSession.Infrastructure.Persistence;

public sealed class OnlineSessionDbContext : DbContext
{
    public OnlineSessionDbContext(DbContextOptions<OnlineSessionDbContext> options) : base(options) { }

    public DbSet<VideoRoom> VideoRooms => Set<VideoRoom>();
    public DbSet<VideoRoomClick> VideoRoomClicks => Set<VideoRoomClick>();
    public DbSet<DefaultVideoProviderSettings> DefaultVideoProviderSettings => Set<DefaultVideoProviderSettings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("online_session");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OnlineSessionDbContext).Assembly);
        modelBuilder.Entity<VideoRoomClick>().ToTable("video_room_clicks").HasKey(entity => entity.Id);
        modelBuilder.Entity<DefaultVideoProviderSettings>().ToTable("default_video_provider_settings").HasKey(entity => entity.Id);
    }
}
