using Core.Domain.Repositories;
using OnlineSession.Domain.Entities;
using OnlineSession.Domain.Filters;

namespace OnlineSession.Domain.Repositories;

public interface IVideoRoomRepository : IRepository<VideoRoom, int, VideoRoomQueryFilter>
{
    Task<VideoRoom?> GetBySessionAndTenantAsync(int sessionId, int tenantId, CancellationToken cancellationToken = default);
    Task<VideoRoom?> GetBySessionAndTenantNoTrackAsync(int sessionId, int tenantId, CancellationToken cancellationToken = default);
}
