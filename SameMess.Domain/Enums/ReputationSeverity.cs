namespace SameMess.Domain.Enums;

/// <summary>
/// Mức nghiêm trọng của sự kiện uy tín — quyết định cách hồi phục:
/// Positive/Severe KHÔNG phai (giữ nguyên), Light phai dần theo thời gian (có thể hồi).
/// </summary>
public static class ReputationSeverity
{
    public const string Positive = "Positive"; // điểm cộng — giữ nguyên
    public const string Light = "Light";       // phạt nhẹ — phai dần (50% sau 30 ngày, hết sau 90)
    public const string Severe = "Severe";     // vi phạm nặng — không tự hồi phục
}
