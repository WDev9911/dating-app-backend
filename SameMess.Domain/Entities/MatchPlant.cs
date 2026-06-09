namespace SameMess.Domain.Entities;

/// <summary>
/// "Cây tình yêu" CHUNG của một cặp match (1-1 với Match, giống Conversation).
/// Cả hai cùng tưới để cây lên level và giữ chuỗi (streak) — chống ghosting.
/// </summary>
public class MatchPlant
{
    public Guid Id { get; set; }
    public Guid MatchId { get; set; }

    /// <summary>Level hiện tại (bắt đầu từ 1).</summary>
    public int Level { get; set; } = 1;

    /// <summary>% tiến độ tới level kế tiếp (0..99). Đủ 100 thì lên level.</summary>
    public int GrowthPercent { get; set; }

    /// <summary>Số ngày giữ chuỗi liên tục.</summary>
    public int StreakCount { get; set; }

    /// <summary>Ngày (UTC) chuỗi được duy trì gần nhất — dùng để chỉ tính chuỗi 1 lần/ngày.</summary>
    public DateOnly? StreakDate { get; set; }

    /// <summary>Ngày (UTC) tương ứng với 2 cờ "đã tưới hôm nay" bên dưới.</summary>
    public DateOnly? WaterDate { get; set; }

    public bool WateredByA { get; set; }
    public bool WateredByB { get; set; }

    /// <summary>Tuần ISO (yyyy-Www) đã dùng quyền "đóng băng" — mỗi tuần tha 1 ngày quên tưới.</summary>
    public string? FreezeUsedWeekKey { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
