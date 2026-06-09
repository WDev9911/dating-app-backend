namespace SameMess.Application.DTOs.Chat;

public class MessageDto
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; } = null!;
    public DateTime SentAt { get; set; }
    public DateTime? ReadAt { get; set; }
}
