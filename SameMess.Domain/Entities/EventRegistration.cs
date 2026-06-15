using SameMess.Domain.Enums;

namespace SameMess.Domain.Entities;

public class EventRegistration
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public Guid UserId { get; set; }
    public string Status { get; set; } = EventRegistrationStatus.Registered;
    public DateTime RegisteredAt { get; set; }

    public Event Event { get; set; } = null!;
}
