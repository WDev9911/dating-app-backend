using SameMess.Application.DTOs.Ai;

namespace SameMess.Application.Interfaces.Services;

/// <summary>
/// Lớp gọi AI thô (hiện dùng Gemini). Trừu tượng hóa để sau đổi nhà cung cấp
/// (OpenAI Moderation / Perspective...) chỉ cần thay implementation.
/// </summary>
public interface IAiAssistantService
{
    /// <summary>Gợi ý 2-3 câu mở lời dựa trên hồ sơ 2 người (tiếng Việt).</summary>
    Task<List<string>> SuggestIcebreakersAsync(string? myBio, string? theirBio, string? theirName);

    /// <summary>Kiểm duyệt nội dung tin nhắn. Fail-open: lỗi AI → coi như không vi phạm.</summary>
    Task<ModerationResult> ModerateMessageAsync(string content);
}
