namespace SameMess.Domain.Entities;

/// <summary>Hội thoại 1-1, gắn với một Match (mỗi match có tối đa một conversation).</summary>
public class Conversation
{
    public Guid Id { get; set; }
    public Guid MatchId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastMessageAt { get; set; }

    public Match Match { get; set; } = null!;
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
