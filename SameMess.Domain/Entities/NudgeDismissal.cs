namespace SameMess.Domain.Entities;

/// <summary>Đánh dấu user đã bỏ qua một gợi ý (nudge) trong một hội thoại.</summary>
public class NudgeDismissal
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ConversationId { get; set; }
    public string NudgeCode { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
