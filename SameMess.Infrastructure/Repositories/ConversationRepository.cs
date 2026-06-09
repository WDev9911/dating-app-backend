using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class ConversationRepository : BaseRepository<Conversation>, IConversationRepository
{
    public ConversationRepository(AppDbContext context) : base(context) { }

    public async Task<Conversation?> GetByMatchIdAsync(Guid matchId) =>
        await _dbSet.FirstOrDefaultAsync(c => c.MatchId == matchId);

    public async Task<Conversation?> GetWithMatchAsync(Guid conversationId) =>
        await _dbSet
            .Include(c => c.Match)
            .FirstOrDefaultAsync(c => c.Id == conversationId);

    public async Task<List<Conversation>> GetForUserAsync(Guid userId) =>
        await _dbSet
            .Include(c => c.Match)
            .Where(c => c.Match.IsActive
                        && (c.Match.UserAId == userId || c.Match.UserBId == userId))
            .OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt)
            .ToListAsync();
}
