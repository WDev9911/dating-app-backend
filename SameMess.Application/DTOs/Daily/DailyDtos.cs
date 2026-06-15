namespace SameMess.Application.DTOs.Daily;

public class DailyQuestDto
{
    public string Code { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int XpReward { get; set; }
    public bool Completed { get; set; }
}

public class DailyConnectionDto
{
    public List<DailyQuestDto> Quests { get; set; } = new();
    public int TotalXp { get; set; }   // tổng XP có thể nhận từ nhiệm vụ hôm nay
    public int UserXp { get; set; }    // tổng XP tích lũy của user
}

public class CompleteDailyDto
{
    public List<string> QuestIds { get; set; } = new();
}

public class CompleteDailyResultDto
{
    public List<string> Completed { get; set; } = new();
    public int XpEarned { get; set; }
    public int TotalXp { get; set; }   // tổng XP tích lũy sau khi cộng
}
