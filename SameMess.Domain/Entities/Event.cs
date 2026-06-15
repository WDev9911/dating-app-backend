namespace SameMess.Domain.Entities;

/// <summary>Sự kiện cộng đồng (offline/online). Quản trị tạo qua admin, user xem & đăng ký.</summary>
public class Event
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public int? Capacity { get; set; }          // null = không giới hạn
    public int XpReward { get; set; }
    public string? Badge { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ICollection<EventRegistration> Registrations { get; set; } = new List<EventRegistration>();
}
