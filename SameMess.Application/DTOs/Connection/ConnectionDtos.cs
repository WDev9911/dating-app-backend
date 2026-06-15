namespace SameMess.Application.DTOs.Connection;

public class ReminderDto
{
    public string Type { get; set; } = null!;          // "say_hi" | "reconnect"
    public Guid MatchId { get; set; }
    public Guid? ConversationId { get; set; }
    public Guid PartnerId { get; set; }
    public string? PartnerName { get; set; }
    public string Message { get; set; } = null!;
    public DateTime? LastActivityAt { get; set; }
}

public class NudgeDto
{
    public string Id { get; set; } = null!;            // mã nudge (dùng để dismiss)
    public Guid ConversationId { get; set; }
    public string Message { get; set; } = null!;
}

public class DismissNudgeDto
{
    public string NudgeId { get; set; } = null!;
}

public class ProposeMeetupDto
{
    public string? VenueId { get; set; }
    public DateTime ProposedAt { get; set; }
    public string? Note { get; set; }
}

public class MeetupResultDto
{
    public Guid MeetupId { get; set; }
    public string Status { get; set; } = null!;
}
