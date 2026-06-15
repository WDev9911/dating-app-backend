namespace SameMess.Application.Daily;

public record DailyQuest(string Code, string Title, string Description, int XpReward);

/// <summary>Danh mục nhiệm vụ hằng ngày cố định (engagement). Sửa ở đây, không cần migration.</summary>
public static class DailyQuestCatalog
{
    public static readonly IReadOnlyList<DailyQuest> All = new[]
    {
        new DailyQuest("daily-login",   "Điểm danh",  "Đăng nhập hôm nay",        10),
        new DailyQuest("daily-swipe",   "Khám phá",   "Vuốt 10 hồ sơ",            15),
        new DailyQuest("daily-message", "Bắt chuyện", "Gửi tin nhắn cho 1 match", 20),
        new DailyQuest("daily-water",   "Chăm cây",   "Tưới cây tình yêu",        15),
        new DailyQuest("daily-profile", "Tỏa sáng",   "Cập nhật hồ sơ",           10),
    };

    public static DailyQuest? Find(string code) => All.FirstOrDefault(q => q.Code == code);

    public static int TotalXp => All.Sum(q => q.XpReward);

    public static string TodayPeriodKey() => DateTime.UtcNow.ToString("yyyyMMdd");
}
