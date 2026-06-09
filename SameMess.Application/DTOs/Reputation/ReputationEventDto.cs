namespace SameMess.Application.DTOs.Reputation;

public class ReputationEventDto
{
    public string Type { get; set; } = null!;
    public int Delta { get; set; }
    public string Severity { get; set; } = null!;
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; }
}
