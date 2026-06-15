namespace SameMess.Application.DTOs.Admin;

public class EventPayloadDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public int? Capacity { get; set; }
    public int XpReward { get; set; }
    public string? Badge { get; set; }
}

public class AdminEventDto
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
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class EventRegistrationDto
{
    public Guid UserId { get; set; }
    public string? DisplayName { get; set; }
    public string Status { get; set; } = null!;
    public DateTime RegisteredAt { get; set; }
}
