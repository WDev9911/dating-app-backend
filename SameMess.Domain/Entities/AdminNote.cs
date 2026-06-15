namespace SameMess.Domain.Entities;

/// <summary>Ghi chú nội bộ của admin về một user.</summary>
public class AdminNote
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid AdminId { get; set; }
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
