using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineSession.Domain.Entities;

namespace PsiFlow.OnlineSession.Infrastructure.Persistence.Configuration;

public sealed class VideoRoomConfiguration : IEntityTypeConfiguration<VideoRoom>
{
    public void Configure(EntityTypeBuilder<VideoRoom> builder)
    {
        builder.ToTable("video_rooms");
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.TenantId).IsRequired();
        builder.Property(entity => entity.Name).HasMaxLength(160).IsRequired();
        builder.Property(entity => entity.Status).HasMaxLength(32).IsRequired();
        builder.Property(entity => entity.CreatedAt).IsRequired();
        builder.Property(entity => entity.UpdatedAt).IsRequired();
    }
}
