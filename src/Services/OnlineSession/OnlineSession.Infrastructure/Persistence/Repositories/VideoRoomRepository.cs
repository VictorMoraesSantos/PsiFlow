using Core.Domain.Filters;
using Core.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using OnlineSession.Domain.Entities;
using OnlineSession.Domain.Filters;
using OnlineSession.Domain.Filters.Specifications;
using OnlineSession.Domain.Repositories;
using PsiFlow.OnlineSession.Infrastructure.Persistence.Data;

namespace OnlineSession.Infrastructure.Persistence.Repositories;

public sealed class VideoRoomRepository(OnlineSessionDbContext dbContext) : Repository<VideoRoom, int, VideoRoomQueryFilter>(dbContext), IVideoRoomRepository
{
    protected override Specification<VideoRoom, int> CreateSpecification(VideoRoomQueryFilter filter) => new VideoRoomSpecification(filter);

    public async Task<VideoRoom?> GetBySessionAndTenantAsync(int sessionId, int tenantId, CancellationToken cancellationToken = default) =>
        await dbContext.VideoRooms.FirstOrDefaultAsync(x => x.SessionId == sessionId && x.TenantId == tenantId, cancellationToken);

    public async Task<VideoRoom?> GetBySessionAndTenantNoTrackAsync(int sessionId, int tenantId, CancellationToken cancellationToken = default) =>
        await dbContext.VideoRooms.AsNoTracking().FirstOrDefaultAsync(x => x.SessionId == sessionId && x.TenantId == tenantId, cancellationToken);
}
