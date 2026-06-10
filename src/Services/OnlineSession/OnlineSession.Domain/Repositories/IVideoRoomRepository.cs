using OnlineSession.Domain.Aggregates;
using OnlineSession.Domain.Filters;
using Core.Domain.Repositories;

namespace OnlineSession.Domain.Repositories
{
    public interface IVideoRoomRepository : IRepository<VideoRoom, int, VideoRoomQueryFilter> { }
}
