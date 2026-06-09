namespace SameMess.Domain.Enums;

/// <summary>Loại nhiệm vụ gamification. Reset theo PeriodKey (không cần cron).</summary>
public static class GameTaskType
{
    public const string Daily = "Daily";         // reset mỗi ngày (yyyy-MM-dd)
    public const string Weekly = "Weekly";       // reset mỗi tuần ISO (yyyy-Www)
    public const string Achievement = "Achievement"; // một lần duy nhất (PeriodKey = "ALL")
}
