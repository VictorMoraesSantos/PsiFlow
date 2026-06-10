using Core.Domain.Filters;

namespace OnlineSession.Domain.Filters;

public class VideoRoomQueryFilter : DomainQuery
{
    public int? TenantId { get; }
    public string? Search { get; }

    public VideoRoomQueryFilter(int? tenantId = null, string? search = null)
    {
        TenantId = tenantId;
        Search = search;
    }
}
