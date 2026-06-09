using SameMess.Domain.Enums;

namespace SameMess.Application.Reputation;

/// <summary>Định nghĩa một loại sự kiện uy tín.</summary>
public record RepEventDef(int Delta, string Severity, bool Once);

/// <summary>
/// Toàn bộ số liệu điểm uy tín để một chỗ (khởi điểm, trần, trọng số, đường phai).
/// Hiện là static code — chuyển sang DB-config nếu cần chỉnh runtime (xem PENDING_FEATURES mục 7).
/// </summary>
public static class ReputationConfig
{
    public const int StartScore = 50;     // khởi điểm trung tính
    public const int UnverifiedCap = 65;  // trần khi CHƯA xác minh khuôn mặt
    public const int MinScore = 0;
    public const int MaxScore = 100;

    /// <summary>
    /// Hệ số phai cho sự kiện LIGHT theo số ngày kể từ lần vi phạm nhẹ GẦN NHẤT
    /// (tái phạm → reset đồng hồ vì mọi sự kiện nhẹ cùng neo vào mốc mới nhất).
    /// 1.0 lúc đầu → 0.5 ở ngày 30 → 0.0 ở ngày 90.
    /// </summary>
    public static double LightDecayFactor(int daysSinceLastLight)
    {
        if (daysSinceLastLight <= 0) return 1.0;
        if (daysSinceLastLight <= 30) return 1.0 - 0.5 * (daysSinceLastLight / 30.0);
        if (daysSinceLastLight <= 90) return 0.5 - 0.5 * ((daysSinceLastLight - 30) / 60.0);
        return 0.0;
    }

    public static readonly IReadOnlyDictionary<string, RepEventDef> Events = new Dictionary<string, RepEventDef>
    {
        [ReputationEventType.ProfileCompleted] = new(+5, ReputationSeverity.Positive, Once: true),
        [ReputationEventType.FaceVerified]     = new(+15, ReputationSeverity.Positive, Once: true),
        [ReputationEventType.GotMatch]         = new(+2, ReputationSeverity.Positive, Once: false),
        [ReputationEventType.Blocked]          = new(-6, ReputationSeverity.Light, Once: false),
        [ReputationEventType.MessageFlagged]   = new(-8, ReputationSeverity.Light, Once: false),
        [ReputationEventType.ReportUpheld]     = new(-20, ReputationSeverity.Severe, Once: false),
    };

    public static string TierOf(int score) =>
        score < 40 ? ReputationTier.New
        : score < 70 ? ReputationTier.Normal
        : score < 90 ? ReputationTier.Good
        : ReputationTier.High;

    public static string TierLabel(string tier) => tier switch
    {
        ReputationTier.New => "⚠️ Mới / cần xác minh",
        ReputationTier.Normal => "🙂 Bình thường",
        ReputationTier.Good => "✅ Uy tín tốt",
        ReputationTier.High => "⭐ Uy tín cao",
        _ => tier,
    };
}
