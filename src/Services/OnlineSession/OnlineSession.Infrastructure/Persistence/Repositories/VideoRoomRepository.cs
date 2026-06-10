using Core.Domain.Filters;
using Core.Infrastructure.Repositories;
using OnlineSession.Domain.Aggregates;
using OnlineSession.Domain.Filters;
using OnlineSession.Domain.Repositories;
using PsiFlow.OnlineSession.Infrastructure.Persistence;

namespace OnlineSession.Infrastructure.Persistence.Repositories;

public sealed class VideoRoomRepository(OnlineSessionDbContext dbContext) : Repository<VideoRoom, int, VideoRoomQueryFilter>(dbContext), IVideoRoomRepository
{
    protected override Specification<VideoRoom, int> CreateSpecification(VideoRoomQueryFilter filter) => new VideoRoomSpecification(filter);

    private sealed class VideoRoomSpecification : Specification<VideoRoom, int>
    {
        public VideoRoomSpecification(VideoRoomQueryFilter filter)
        {
            ApplyBaseFilters(filter);
            AddIf(filter.TenantId.HasValue, x => x.TenantId == filter.TenantId!.Value);
            AddIf(!string.IsNullOrWhiteSpace(filter.Search), x => x.Name.Contains(filter.Search!) || x.Provider.Contains(filter.Search!));
        }
    }
}
