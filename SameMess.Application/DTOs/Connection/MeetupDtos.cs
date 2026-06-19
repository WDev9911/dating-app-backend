namespace SameMess.Application.DTOs.Connection;

public class VenueDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Category { get; set; } = null!;
    public string? Address { get; set; }
    public string? District { get; set; }
    public string City { get; set; } = null!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? ImageUrl { get; set; }
    public string? PriceRange { get; set; }
    public string? Description { get; set; }
    public int DistanceKm { get; set; }   // khoảng cách tới điểm giữa 2 người (làm tròn)
}

public class RespondMeetupDto
{
    public string Action { get; set; } = null!;   // "accept" | "decline"
}

public class MeetupDto
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Guid ProposerId { get; set; }
    public bool IsMine { get; set; }              // true nếu chính tôi là người đề xuất
    public Guid? VenueId { get; set; }
    public string? VenueName { get; set; }
    public DateTime ProposedAt { get; set; }
    public string? Note { get; set; }
    public string Status { get; set; } = null!;   // Proposed | Accepted | Declined
    public DateTime CreatedAt { get; set; }
}
