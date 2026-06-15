namespace SameMess.Application.DTOs.Events;

public class EventDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public int? Capacity { get; set; }
    public int RegisteredCount { get; set; }
    public int XpReward { get; set; }
    public string? Badge { get; set; }
    public bool IsRegistered { get; set; }
}
