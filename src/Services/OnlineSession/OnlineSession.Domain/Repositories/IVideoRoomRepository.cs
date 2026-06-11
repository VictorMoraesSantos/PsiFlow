using Core.Domain.Repositories;
using OnlineSession.Domain.Entities;
using OnlineSession.Domain.Filters;

namespace OnlineSession.Domain.Repositories
{
    public interface IVideoRoomRepository : IRepository<VideoRoom, int, VideoRoomQueryFilter> { }
}
