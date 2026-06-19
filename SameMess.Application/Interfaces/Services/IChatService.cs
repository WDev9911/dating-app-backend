using SameMess.Application.DTOs.Chat;

namespace SameMess.Application.Interfaces.Services;

public interface IChatService
{
    Task<List<ConversationDto>> GetConversationsAsync(Guid userId);

    /// <summary>Lấy (hoặc tạo) conversation cho một match mà user là thành viên.</summary>
    Task<ConversationDto> GetOrCreateByMatchAsync(Guid userId, Guid matchId);

    Task<List<MessageDto>> GetMessagesAsync(Guid userId, Guid conversationId, Guid? beforeMessageId, int limit);

    /// <summary>Gửi tin nhắn; trả về tin đã lưu và id của người nhận (để bắn realtime).</summary>
    Task<(MessageDto Message, Guid OtherUserId)> SendMessageAsync(Guid userId, Guid conversationId, string content);

    /// <summary>Chia sẻ một địa điểm vào hội thoại dưới dạng "thẻ quán" (Type = venue).</summary>
    Task<(MessageDto Message, Guid OtherUserId)> ShareVenueAsync(Guid userId, Guid conversationId, Guid venueId);

    Task MarkReadAsync(Guid userId, Guid conversationId);

    /// <summary>Id người còn lại trong conversation (đồng thời xác thực user là thành viên).</summary>
    Task<Guid> GetOtherParticipantAsync(Guid userId, Guid conversationId);
}
