namespace SameMess.Domain.Enums;

/// <summary>
/// Hành vi người dùng kích hoạt tiến độ nhiệm vụ. Các service sẵn có gọi
/// ITaskService.RecordActionAsync(userId, action) với một trong các hằng này.
/// </summary>
public static class GameAction
{
    public const string Login = "Login";                   // mở app / xem nhiệm vụ
    public const string Swipe = "Swipe";                   // quẹt một người
    public const string Match = "Match";                   // có match mới
    public const string SendMessage = "SendMessage";       // gửi tin nhắn
    public const string Water = "Water";                   // tưới cây
    public const string CompleteProfile = "CompleteProfile"; // hoàn thiện hồ sơ
}
