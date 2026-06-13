using Core.Application.DTOs;

namespace OnlineSession.Application.DTOs.VideoRoom
{
    public sealed record VideoRoomDTO(
        int Id,
        int TenantId,
        int SessionId,
        string Name,
        string Provider,
        string UrlEncrypted,
        string UrlHash,
        string? Instructions,
        int CreatedBy,
        string Status,
        DateTime CreatedAt,
        DateTime? UpdatedAt) : DTOBase<int>(Id, CreatedAt, UpdatedAt);

    public sealed record CreateVideoRoomDTO(
        int TenantId,
        int SessionId,
        string Name,
        string Provider = "external",
        string UrlEncrypted = "",
        string UrlHash = "",
        string? Instructions = null,
        int CreatedBy = default,
        string Status = "active");

    public sealed record UpdateVideoRoomDTO(
        int Id,
        int TenantId,
        int SessionId,
        string Name,
        string Provider,
        string UrlEncrypted,
        string UrlHash,
        string? Instructions,
        int CreatedBy,
        string Status);

    public sealed record VideoRoomFilterDTO(
        int? TenantId,
        string? Search,
        int? Page = 1,
        int? PageSize = 20) : DomainQueryFilterDto(Page, PageSize);
}
