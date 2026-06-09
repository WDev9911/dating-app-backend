namespace SameMess.Domain.Enums;

/// <summary>Loại sự kiện làm thay đổi điểm uy tín (event-driven, hook vào các service sẵn có).</summary>
public static class ReputationEventType
{
    // 🟢 Cộng
    public const string ProfileCompleted = "ProfileCompleted"; // hoàn thiện hồ sơ (1 lần)
    public const string FaceVerified = "FaceVerified";         // xác minh khuôn mặt (1 lần) — GỠ TRẦN 65
    public const string GotMatch = "GotMatch";                 // có match mới

    // 🔴 Trừ
    public const string Blocked = "Blocked";               // bị người khác block (nhẹ)
    public const string MessageFlagged = "MessageFlagged"; // tin nhắn bị AI kiểm duyệt gắn cờ (nhẹ)
    public const string ReportUpheld = "ReportUpheld";     // bị report và admin xử lý (nặng)
}
