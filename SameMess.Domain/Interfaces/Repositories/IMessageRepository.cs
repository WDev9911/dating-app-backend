using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IMessageRepository : IBaseRepository<Message>
{
    /// <summary>Một trang tin nhắn (cũ hơn beforeMessageId nếu có), trả về theo thứ tự tăng dần thời gian.</summary>
    Task<List<Message>> GetPageAsync(Guid conversationId, Guid? beforeMessageId, int limit);

    Task<Message?> GetLastAsync(Guid conversationId);

    /// <summary>Số tin chưa đọc trong conversation (tin do người khác gửi, chưa có ReadAt).</summary>
    Task<int> CountUnreadAsync(Guid conversationId, Guid readerId);

    /// <summary>Đánh dấu đã đọc mọi tin do người khác gửi trong conversation.</summary>
    Task MarkReadAsync(Guid conversationId, Guid readerId);
}
