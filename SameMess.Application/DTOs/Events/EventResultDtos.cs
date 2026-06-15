namespace SameMess.Application.DTOs.Events;

public class RegisterResultDto
{
    public Guid RegistrationId { get; set; }
    public string Status { get; set; } = null!;
}

public class EventRewardDto
{
    public int Xp { get; set; }
    public string? Badge { get; set; }
}
