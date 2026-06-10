using Notifications.Application.DTOs.NotificationTemplate;
using Notifications.Domain.Aggregates;

namespace Notifications.Application.Mapping
{
    public static class NotificationTemplateMapper
    {
        public static NotificationTemplateDTO ToDTO(this NotificationTemplate entity) => new(
            entity.Id,
            entity.TenantId,
            entity.Key,
            entity.Channel,
            entity.Name,
            entity.Status,
            entity.IsActive,
            entity.CreatedAt,
            entity.UpdatedAt);

        public static NotificationTemplate ToEntity(this CreateNotificationTemplateDTO dto) => new()
        {
            TenantId = dto.TenantId,
            Key = dto.Key,
            Channel = dto.Channel,
            Name = dto.Name,
            Status = dto.Status,
            IsActive = dto.IsActive
        };
    }
}
