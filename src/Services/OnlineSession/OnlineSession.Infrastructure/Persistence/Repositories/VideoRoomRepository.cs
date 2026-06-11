using Core.Domain.Filters;
using Core.Infrastructure.Repositories;
using OnlineSession.Domain.Entities;
using OnlineSession.Domain.Filters;
using OnlineSession.Domain.Filters.Specifications;
using OnlineSession.Domain.Repositories;
using PsiFlow.OnlineSession.Infrastructure.Persistence.Data;

namespace OnlineSession.Infrastructure.Persistence.Repositories;

public sealed class VideoRoomRepository(OnlineSessionDbContext dbContext) : Repository<VideoRoom, int, VideoRoomQueryFilter>(dbContext), IVideoRoomRepository
{
    protected override Specification<VideoRoom, int> CreateSpecification(VideoRoomQueryFilter filter) => new VideoRoomSpecification(filter);
}
