namespace SameMess.Application.DTOs.Gamification;

public class WaterResultDto
{
    public int Level { get; set; }
    public int GrowthPercent { get; set; }
    public int StreakCount { get; set; }
    public bool LeveledUp { get; set; }
    public bool BonusApplied { get; set; }      // cả hai cùng tưới hôm nay → nhân đôi %
    public bool MilestoneReached { get; set; }  // level vừa đạt là mốc thưởng cặp đôi
    public string Message { get; set; } = string.Empty;
}
