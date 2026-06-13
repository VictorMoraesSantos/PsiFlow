using Auth.Domain.Entities;
using Auth.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Infrastructure.Persistence.Configuration
{
    public class ConsentConfiguration : IEntityTypeConfiguration<Consent>
    {
        public void Configure(EntityTypeBuilder<Consent> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasConversion(
                    id => id.Value,
                    value => new ConsentId(value))
                .ValueGeneratedOnAdd();
            builder.Property(x => x.DocumentType).IsRequired().HasMaxLength(64);
            builder.Property(x => x.Version).IsRequired().HasMaxLength(64);
            builder.Property(x => x.DocumentHash).IsRequired().HasMaxLength(256);
            builder.Property(x => x.IpAddress).HasMaxLength(64);
            builder.Property(x => x.UserAgent).HasMaxLength(256);
        }
    }
}
