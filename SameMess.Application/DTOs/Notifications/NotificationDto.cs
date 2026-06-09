namespace SameMess.Application.DTOs.Notifications;

public class NotificationDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;
    public string? Data { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class NotificationFeedDto
{
    public int UnreadCount { get; set; }
    public List<NotificationDto> Items { get; set; } = new();
}
