using System.Globalization;
using SameMess.Domain.Enums;

namespace SameMess.Application.Gamification;

/// <summary>Định nghĩa một nhiệm vụ gamification (catalog tĩnh, tập trung số liệu để dễ chỉnh balance).</summary>
public record GameTask(
    string Code,
    string Type,            // GameTaskType
    string Action,          // GameAction kích hoạt tiến độ
    int Target,
    string RewardMaterial,  // PlantMaterial thưởng khi hoàn thành
    int RewardQty,
    string Description);

/// <summary>
/// Toàn bộ số liệu cân bằng của "Cây tình yêu" để một chỗ — chỉnh balance không phải đi tìm khắp nơi.
/// (Bước sau có thể chuyển sang config/DB nếu cần đổi runtime — xem PENDING_FEATURES mục 6.)
/// </summary>
public static class GamificationConfig
{
    public const int PercentPerLevel = 100;

    /// <summary>Khi CẢ HAI cùng tưới trong ngày, lần tưới "chốt cặp" được nhân % này (thưởng bonus).</summary>
    public const int BothWateredBonusMultiplier = 2;

    /// <summary>Mỗi tuần được "đóng băng" tha 1 ngày quên tưới mà không đứt chuỗi.</summary>
    public const int FreezeGraceDays = 1;

    /// <summary>% lên cây mỗi lần tưới — phân bón hiếm nên cộng nhiều nhất.</summary>
    public static int GrowthPercent(string material) => material switch
    {
        PlantMaterial.Water => 5,
        PlantMaterial.Sun => 12,
        PlantMaterial.Fertilizer => 25,
        _ => 0,
    };

    /// <summary>Các mốc level có thưởng cặp đôi (badge/theme + perk nhẹ).</summary>
    public static readonly int[] MilestoneLevels = { 5, 10, 20, 50 };

    public static readonly IReadOnlyList<GameTask> Tasks = new List<GameTask>
    {
        new("DAILY_CHECKIN",        GameTaskType.Daily,       GameAction.Login,           1,  PlantMaterial.Water,      1, "Mở app hôm nay"),
        new("DAILY_SWIPE_10",       GameTaskType.Daily,       GameAction.Swipe,           10, PlantMaterial.Water,      1, "Quẹt 10 người"),
        new("DAILY_MESSAGE",        GameTaskType.Daily,       GameAction.SendMessage,     1,  PlantMaterial.Sun,        1, "Gửi tin nhắn cho match"),
        new("DAILY_WATER",          GameTaskType.Daily,       GameAction.Water,           1,  PlantMaterial.Water,      1, "Tưới cây 1 lần"),
        new("WEEKLY_MATCH_3",       GameTaskType.Weekly,      GameAction.Match,           3,  PlantMaterial.Fertilizer, 1, "Match 3 người trong tuần"),
        new("WEEKLY_WATER_5",       GameTaskType.Weekly,      GameAction.Water,           5,  PlantMaterial.Fertilizer, 1, "Tưới cây 5 lần trong tuần"),
        new("ACH_COMPLETE_PROFILE", GameTaskType.Achievement, GameAction.CompleteProfile, 1,  PlantMaterial.Fertilizer, 2, "Hoàn thiện hồ sơ"),
        new("ACH_FIRST_MATCH",      GameTaskType.Achievement, GameAction.Match,           1,  PlantMaterial.Sun,        2, "Có match đầu tiên"),
    };

    /// <summary>Khóa kỳ theo loại nhiệm vụ: daily=ngày, weekly=tuần ISO, achievement="ALL" (không reset).</summary>
    public static string PeriodKey(string type, DateOnly date) => type switch
    {
        GameTaskType.Daily => date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
        GameTaskType.Weekly => WeekKey(date),
        _ => "ALL",
    };

    public static string WeekKey(DateOnly date)
    {
        var dt = date.ToDateTime(TimeOnly.MinValue);
        var year = ISOWeek.GetYear(dt);
        var week = ISOWeek.GetWeekOfYear(dt);
        return $"{year}-W{week:D2}";
    }

    public static bool IsMilestone(int level) => MilestoneLevels.Contains(level);
}
