using Auth.Domain.Entities;
using Auth.Domain.ValueObjects;
using Core.Domain.Filters;

namespace Auth.Domain.Filters.Specifications
{
    public class UserSpecification : Specification<User, UserId>
    {
        public UserSpecification(UserFilter filter)
        {
            ApplyBaseFilters(filter);
            AddIf(filter.RestaurantId.HasValue, u => u.RestaurantId == filter.RestaurantId!.Value);
            AddIf(filter.UnitId.HasValue, u => u.UnitId == filter.UnitId!.Value);
            AddIf(!string.IsNullOrWhiteSpace(filter.Email), u => u.Email == filter.Email!.Trim().ToLowerInvariant());
            AddIf(!string.IsNullOrWhiteSpace(filter.NameContains), u => u.Name.FirstName.Contains(filter.NameContains!.Trim()) || u.Name.LastName.Contains(filter.NameContains!.Trim()));
            AddIf(filter.IsActive.HasValue, u => u.IsActive == filter.IsActive!.Value);
            AddIf(filter.Birthday.HasValue, u => u.Birthday == filter.Birthday!.Value);
        }
    }
}
