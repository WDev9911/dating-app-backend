namespace SameMess.Application.DTOs.Chat;

public class ConversationDto
{
    public Guid Id { get; set; }
    public Guid MatchId { get; set; }

    public Guid OtherUserId { get; set; }
    public string OtherDisplayName { get; set; } = null!;
    public string? OtherAvatarUrl { get; set; }
    public bool OtherIsAdmin { get; set; }
    public string? OtherAvatarFrame { get; set; }

    public string? LastMessageText { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public int UnreadCount { get; set; }
}
