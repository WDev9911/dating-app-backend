using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class MessageRepository : BaseRepository<Message>, IMessageRepository
{
    public MessageRepository(AppDbContext context) : base(context) { }

    public async Task<List<Message>> GetPageAsync(Guid conversationId, Guid? beforeMessageId, int limit)
    {
        var query = _dbSet.AsNoTracking().Where(m => m.ConversationId == conversationId);

        if (beforeMessageId is { } beforeId)
        {
            var before = await _dbSet.AsNoTracking().FirstOrDefaultAsync(m => m.Id == beforeId);
            if (before is not null)
                query = query.Where(m => m.SentAt < before.SentAt);
        }

        // Lấy `limit` tin mới nhất (theo điều kiện), rồi đảo về thứ tự tăng dần để hiển thị
        var page = await query
            .OrderByDescending(m => m.SentAt)
            .Take(limit)
            .ToListAsync();

        page.Reverse();
        return page;
    }

    public async Task<Message?> GetLastAsync(Guid conversationId) =>
        await _dbSet.AsNoTracking()
            .Where(m => m.ConversationId == conversationId)
            .OrderByDescending(m => m.SentAt)
            .FirstOrDefaultAsync();

    public async Task<int> CountUnreadAsync(Guid conversationId, Guid readerId) =>
        await _dbSet
            .CountAsync(m => m.ConversationId == conversationId
                             && m.SenderId != readerId
                             && m.ReadAt == null);

    public async Task MarkReadAsync(Guid conversationId, Guid readerId)
    {
        var now = DateTime.UtcNow;
        await _dbSet
            .Where(m => m.ConversationId == conversationId
                        && m.SenderId != readerId
                        && m.ReadAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(m => m.ReadAt, now));
    }
}
