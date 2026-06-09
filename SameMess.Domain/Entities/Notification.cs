namespace SameMess.Domain.Entities;

/// <summary>
/// Một thông báo gửi tới user (vừa là feed trong app, vừa là "outbox" cho push).
/// Realtime khi online qua SignalR; thông báo này phục vụ xem lại + badge chưa đọc + push offline.
/// </summary>
public class Notification
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Type { get; set; } = null!;     // NotificationType
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;
    public string? Data { get; set; }              // ngữ cảnh (matchId/conversationId/fromUserId...)
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}
