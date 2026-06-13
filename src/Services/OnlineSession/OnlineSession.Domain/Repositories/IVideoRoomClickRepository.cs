using Core.Domain.Repositories;
using OnlineSession.Domain.Entities;

namespace OnlineSession.Domain.Repositories;

public interface IVideoRoomClickRepository : IRepository<VideoRoomClick, int>
{
    Task<IReadOnlyCollection<VideoRoomClick>> ListBySessionOrderedDescAsync(int sessionId, int tenantId, CancellationToken cancellationToken = default);
}
