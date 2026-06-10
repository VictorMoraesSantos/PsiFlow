using Core.Domain.Aggregates;

namespace OnlineSession.Domain.Aggregates;

public class VideoRoomClick : BaseEntity<int>
{
    public int TenantId { get; set; }
    public int VideoRoomId { get; set; }
    public int SessionId { get; set; }
    public int? ClickedByUserId { get; set; }
    public string? ClickedByRole { get; set; }
    public DateTime ClickedAt { get; set; } = DateTime.UtcNow;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public int? CorrelationId { get; set; }
}
