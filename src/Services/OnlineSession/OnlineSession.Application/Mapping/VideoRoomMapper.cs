using OnlineSession.Application.DTOs.VideoRoom;
using OnlineSession.Domain.Aggregates;

namespace OnlineSession.Application.Mapping
{
    public static class VideoRoomMapper
    {
        public static VideoRoomDTO ToDTO(this VideoRoom entity) => new(
            entity.Id,
            entity.TenantId,
            entity.SessionId,
            entity.Name,
            entity.Provider,
            entity.UrlEncrypted,
            entity.UrlHash,
            entity.Instructions,
            entity.CreatedBy,
            entity.Status,
            entity.CreatedAt,
            entity.UpdatedAt);

        public static VideoRoom ToEntity(this CreateVideoRoomDTO dto) => new()
        {
            TenantId = dto.TenantId,
            SessionId = dto.SessionId,
            Name = dto.Name,
            Provider = dto.Provider,
            UrlEncrypted = dto.UrlEncrypted,
            UrlHash = dto.UrlHash,
            Instructions = dto.Instructions,
            CreatedBy = dto.CreatedBy,
            Status = dto.Status
        };
    }
}
