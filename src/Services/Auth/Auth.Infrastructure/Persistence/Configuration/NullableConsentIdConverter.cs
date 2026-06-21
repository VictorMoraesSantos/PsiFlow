using Auth.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Auth.Infrastructure.Persistence.Configuration
{
    public class NullableConsentIdConverter : ValueConverter<ConsentId?, int?>
    {
        public NullableConsentIdConverter()
            : base(
                id => id != null ? id.Value : null,
                value => value.HasValue ? new ConsentId(value.Value) : null)
        {
        }
    }
}
