namespace SameMess.Domain.Entities;

public class Message
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; } = null!;
    public string Type { get; set; } = Enums.MessageType.Text;   // text | venue
    public Guid? VenueId { get; set; }                            // có khi Type = venue
    public DateTime SentAt { get; set; }
    public DateTime? ReadAt { get; set; }

    public Conversation Conversation { get; set; } = null!;
}
