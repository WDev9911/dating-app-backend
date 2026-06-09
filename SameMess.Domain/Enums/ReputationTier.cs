namespace SameMess.Domain.Enums;

/// <summary>
/// Mức uy tín hiển thị (người khác CHỈ thấy mức/badge, không thấy số điểm).
/// 0–40 New, 40–70 Normal, 70–90 Good, 90–100 High.
/// </summary>
public static class ReputationTier
{
    public const string New = "New";       // ⚠️ Mới / cần xác minh
    public const string Normal = "Normal"; // 🙂 Bình thường
    public const string Good = "Good";     // ✅ Uy tín tốt
    public const string High = "High";     // ⭐ Uy tín cao
}
