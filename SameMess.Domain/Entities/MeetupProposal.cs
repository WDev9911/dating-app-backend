using SameMess.Domain.Enums;

namespace SameMess.Domain.Entities;

/// <summary>Một đề xuất gặp mặt trong hội thoại.</summary>
public class MeetupProposal
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Guid ProposerId { get; set; }
    public string? VenueId { get; set; }       // text/id địa điểm (chưa có bảng Venue)
    public DateTime ProposedAt { get; set; }
    public string? Note { get; set; }
    public string Status { get; set; } = MeetupStatus.Proposed;
    public DateTime CreatedAt { get; set; }
}
