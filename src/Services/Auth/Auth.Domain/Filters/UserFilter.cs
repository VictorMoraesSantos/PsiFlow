using Core.Domain.Filters;

namespace Auth.Domain.Filters
{
    public class UserFilter : DomainQuery
    {
        public int? RestaurantId { get; private set; }
        public int? UnitId { get; private set; }
        public string? Email { get; private set; }
        public string? NameContains { get; private set; }
        public bool? IsActive { get; private set; }
        public DateTime? Birthday { get; private set; }

        public UserFilter(
            int? restaurantId = null,
            int? unitId = null,
            string? email = null,
            string? nameContains = null,
            bool? isActive = null,
            DateTime? birthday = null)
        {
            RestaurantId = restaurantId;
            UnitId = unitId;
            Email = email;
            NameContains = nameContains;
            IsActive = isActive;
            Birthday = birthday;
        }
    }
}
