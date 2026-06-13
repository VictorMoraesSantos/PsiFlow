using Core.Application.DTOs;

namespace Notifications.Application.DTOs.NotificationTemplate
{
    public sealed record NotificationTemplateDTO(
        int Id,
        int? TenantId,
        string Key,
        string Channel,
        string Name,
        string Status,
        bool IsActive,
        DateTime CreatedAt,
        DateTime? UpdatedAt) : DTOBase<int>(Id, CreatedAt, UpdatedAt);

    public sealed record CreateNotificationTemplateDTO(
        int? TenantId,
        string Key,
        string Channel = "email",
        string Name = "",
        string Status = "active",
        bool IsActive = true);

    public sealed record UpdateNotificationTemplateDTO(
        int Id,
        int? TenantId,
        string Key,
        string Channel,
        string Name,
        string Status,
        bool IsActive);

    public sealed record NotificationTemplateFilterDTO(
        int? TenantId,
        string? Search,
        int? Page = 1,
        int? PageSize = 20) : DomainQueryFilterDto(Page, PageSize);
}
