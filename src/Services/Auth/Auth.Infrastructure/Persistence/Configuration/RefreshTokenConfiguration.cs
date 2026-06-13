using Auth.Domain.Entities;
using Auth.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Infrastructure.Persistence.Configuration
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasConversion(
                    id => id.Value,
                    value => new RefreshTokenId(value))
                .ValueGeneratedOnAdd();
            builder.Property(x => x.TokenHash).IsRequired().HasMaxLength(256);
            builder.Property(x => x.CreatedByIp).HasMaxLength(64);
            builder.Property(x => x.RevokedByIp).HasMaxLength(64);
            builder.Property(x => x.UserAgent).HasMaxLength(256);
            builder.Property(x => x.ReplacedByTokenId)
                .HasConversion(v => v == null ? 0 : v.Value, v => v == 0 ? null : new RefreshTokenId(v));
            builder.HasIndex(x => x.TokenHash).IsUnique();
            builder.HasIndex(x => new { x.UserId, x.RevokedAt });
        }
    }
}
