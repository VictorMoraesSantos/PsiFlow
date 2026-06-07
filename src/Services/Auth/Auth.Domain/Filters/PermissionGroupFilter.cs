using Core.Domain.Filters;

namespace Auth.Domain.Filters
{
    public class PermissionGroupFilter : DomainQuery
    {
        public string? GroupKeyContains { get; set; }
        public string? GroupNameContains { get; private set; }
        public string? DescriptionContains { get; private set; }
        public bool? IsActive { get; private set; }

        public PermissionGroupFilter(
            string? groupKeyContains = null,
            string? groupNameContains = null,
            string? descriptionContains = null,
            bool? isActive = null)
        {
            GroupKeyContains = groupKeyContains;
            GroupNameContains = groupNameContains;
            DescriptionContains = descriptionContains;
            IsActive = isActive;
        }
    }
}
