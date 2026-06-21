using Auth.Domain.Entities;
using Auth.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Infrastructure.Persistence.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasConversion(
                    id => id.Value,
                    value => new UserId(value))
                .ValueGeneratedOnAdd();

            builder.Property(x => x.TenantId)
                .HasConversion(
                    id => id.Value,
                    value => new TenantId(value));

            builder.Property(x => x.CurrentConsentId)
                .HasConversion(new NullableConsentIdConverter());

            builder.OwnsOne(x => x.Name, name =>
            {
                name.Property(x => x.FirstName).HasColumnName("FirstName");
                name.Property(x => x.LastName).HasColumnName("LastName");
                name.Ignore(x => x.FullName);
            });

            builder.OwnsOne(x => x.Contact, contact =>
            {
                contact.Property(x => x.Email).HasColumnName("ContactEmail");
                contact.Property(x => x.Phone).HasColumnName("Phone");
            });
        }
    }
}
