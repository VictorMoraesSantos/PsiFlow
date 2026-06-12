using Core.Domain.Filters;

namespace Auth.Domain.Filters
{
    public class PermissionGroupFilter : DomainQuery
    {
        public string? GroupKey { get; private set; }
        public string? GroupNameContains { get; private set; }
        public bool? IsActive { get; private set; }

        public PermissionGroupFilter(
            string? groupKey = null,
            string? groupNameContains = null,
            bool? isActive = null)
        {
            GroupKey = groupKey;
            GroupNameContains = groupNameContains;
            IsActive = isActive;
        }
    }
}
