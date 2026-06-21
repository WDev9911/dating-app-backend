namespace SameMess.Application.DTOs.Gamification;

public class TaskDto
{
    public string Code { get; set; } = null!;
    public string Type { get; set; } = null!;       // Daily / Weekly / Achievement
    public string Description { get; set; } = null!;
    public int Target { get; set; }
    public int Progress { get; set; }
    public bool Completed { get; set; }
    public bool Claimed { get; set; }
    public string RewardMaterial { get; set; } = null!;
    public int RewardQty { get; set; }
}
