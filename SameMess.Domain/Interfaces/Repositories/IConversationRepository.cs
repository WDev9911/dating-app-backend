using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IConversationRepository : IBaseRepository<Conversation>
{
    Task<Conversation?> GetByMatchIdAsync(Guid matchId);

    /// <summary>Lấy conversation kèm Match (để kiểm tra thành viên).</summary>
    Task<Conversation?> GetWithMatchAsync(Guid conversationId);

    /// <summary>Các conversation của user (match còn active), sắp theo tin nhắn mới nhất.</summary>
    Task<List<Conversation>> GetForUserAsync(Guid userId);
}
