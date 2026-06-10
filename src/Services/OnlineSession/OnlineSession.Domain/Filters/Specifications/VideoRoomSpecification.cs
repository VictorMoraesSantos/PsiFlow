using Core.Domain.Filters;
using OnlineSession.Domain.Aggregates;

namespace OnlineSession.Domain.Filters.Specifications;

public sealed class VideoRoomSpecification : Specification<VideoRoom, int>
{
    public VideoRoomSpecification(VideoRoomQueryFilter filter)
    {
        ApplyBaseFilters(filter);
        AddIf(filter.TenantId.HasValue, x => x.TenantId == filter.TenantId!.Value);
        AddIf(!string.IsNullOrWhiteSpace(filter.Search), x => x.Name.Contains(filter.Search!) || x.Provider.Contains(filter.Search!));
    }
}
