using Microsoft.EntityFrameworkCore;
using OnlineSession.Domain.Entities;
using OnlineSession.Domain.Repositories;
using PsiFlow.OnlineSession.Infrastructure.Persistence.Data;
using System.Linq.Expressions;

namespace OnlineSession.Infrastructure.Persistence.Repositories;

public sealed class VideoRoomClickRepository(OnlineSessionDbContext dbContext) : IVideoRoomClickRepository
{
    public async Task<VideoRoomClick?> GetById(int id, CancellationToken cancellationToken = default) =>
        await dbContext.VideoRoomClicks.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IEnumerable<VideoRoomClick?>> GetAll(CancellationToken cancellationToken = default) =>
        await dbContext.VideoRoomClicks.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IEnumerable<VideoRoomClick?>> Find(Expression<Func<VideoRoomClick, bool>> predicate, CancellationToken cancellationToken = default) =>
        await dbContext.VideoRoomClicks.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public async Task Create(VideoRoomClick entity, CancellationToken cancellationToken = default) =>
        await dbContext.VideoRoomClicks.AddAsync(entity, cancellationToken);

    public async Task CreateRange(IEnumerable<VideoRoomClick> entities, CancellationToken cancellationToken = default) =>
        await dbContext.VideoRoomClicks.AddRangeAsync(entities, cancellationToken);

    public async Task Update(VideoRoomClick entity, CancellationToken cancellationToken = default) =>
        dbContext.VideoRoomClicks.Update(entity);

    public async Task Delete(VideoRoomClick entity, CancellationToken cancellationToken = default) =>
        dbContext.VideoRoomClicks.Remove(entity);

    public async Task<IReadOnlyCollection<VideoRoomClick>> ListBySessionOrderedDescAsync(int sessionId, int tenantId, CancellationToken cancellationToken = default) =>
        await dbContext.VideoRoomClicks.AsNoTracking()
            .Where(x => x.SessionId == sessionId && x.TenantId == tenantId)
            .OrderByDescending(x => x.ClickedAt)
            .ToListAsync(cancellationToken);
}
