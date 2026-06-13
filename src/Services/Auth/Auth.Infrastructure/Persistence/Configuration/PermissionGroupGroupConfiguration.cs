using Auth.Domain.Entities;
using Auth.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Infrastructure.Persistence.Configuration
{
    public class PermissionGroupConfiguration : IEntityTypeConfiguration<PermissionGroup>
    {
        public void Configure(EntityTypeBuilder<PermissionGroup> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasConversion(
                    id => id.Value,
                    value => new PermissionGroupId(value))
                .ValueGeneratedOnAdd();

            builder.HasMany(x => x.Permissions)
                .WithOne(x => x.PermissionGroup)
                .HasForeignKey(x => x.PermissionGroupId);
        }
    }
}
