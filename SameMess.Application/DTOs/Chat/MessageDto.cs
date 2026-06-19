namespace SameMess.Application.DTOs.Chat;

public class MessageDto
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; } = null!;
    public string Type { get; set; } = "text";       // text | venue
    public DateTime SentAt { get; set; }
    public DateTime? ReadAt { get; set; }

    // Chỉ có khi Type = "venue" (để render thẻ quán; bấm vào dùng VenueId xem chi tiết)
    public Guid? VenueId { get; set; }
    public string? VenueName { get; set; }
    public string? VenueImageUrl { get; set; }
    public string? VenueAddress { get; set; }
    public string? VenueCategory { get; set; }
}

public class ShareVenueDto
{
    public Guid VenueId { get; set; }
}
