namespace SameMess.Application.DTOs.Gamification;

/// <summary>Trạng thái cây chung của một cặp match.</summary>
public class PlantDto
{
    public Guid MatchId { get; set; }
    public int Level { get; set; }
    public int GrowthPercent { get; set; }      // 0..99 tiến độ tới level kế
    public int PercentPerLevel { get; set; }    // = 100, để client vẽ thanh tiến độ
    public int StreakCount { get; set; }
    public bool IWateredToday { get; set; }
    public bool BothWateredToday { get; set; }
}
