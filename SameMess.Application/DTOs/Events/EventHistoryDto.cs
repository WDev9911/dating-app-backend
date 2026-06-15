namespace SameMess.Application.DTOs.Events;

public class EventHistoryDto
{
    public Guid EventId { get; set; }
    public string Title { get; set; } = null!;
    public DateTime StartAt { get; set; }
    public string Status { get; set; } = null!;
    public DateTime RegisteredAt { get; set; }
    public int XpEarned { get; set; }
    public string? Badge { get; set; }
}
