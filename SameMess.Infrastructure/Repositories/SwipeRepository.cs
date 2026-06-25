using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Enums;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class SwipeRepository : BaseRepository<Swipe>, ISwipeRepository
{
    public SwipeRepository(AppDbContext context) : base(context) { }

    public async Task<Swipe?> GetAsync(Guid swiperId, Guid targetUserId) =>
        await _dbSet.FirstOrDefaultAsync(s => s.SwiperId == swiperId && s.TargetUserId == targetUserId);

    public async Task<List<Guid>> GetSwipedTargetIdsAsync(Guid swiperId) =>
        await _dbSet.Where(s => s.SwiperId == swiperId)
            .Select(s => s.TargetUserId)
            .ToListAsync();

    public async Task<List<Swipe>> GetLikersAsync(Guid userId)
    {
        // Chỉ ẩn những người mình đã Like/SuperLike (đã thành / sắp thành match).
        // Người mình từng Pass vẫn hiển thị nếu họ thích mình — để có thể đổi ý.
        var myLikedTargets = _dbSet
            .Where(s => s.SwiperId == userId
                        && (s.Action == SwipeAction.Like || s.Action == SwipeAction.SuperLike))
            .Select(s => s.TargetUserId);

        return await _dbSet
            .Where(s => s.TargetUserId == userId
                        && s.Action == SwipeAction.Like
                        && !myLikedTargets.Contains(s.SwiperId))
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Swipe>> GetSuperLikersAsync(Guid userId)
    {
        var myLikedTargets = _dbSet
            .Where(s => s.SwiperId == userId
                        && (s.Action == SwipeAction.Like || s.Action == SwipeAction.SuperLike))
            .Select(s => s.TargetUserId);

        return await _dbSet
            .Where(s => s.TargetUserId == userId
                        && s.Action == SwipeAction.SuperLike
                        && !myLikedTargets.Contains(s.SwiperId))
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<Swipe?> GetLastSwipeAsync(Guid userId) =>
        await _dbSet.Where(s => s.SwiperId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync();

    public async Task<int> CountLikesSinceAsync(Guid userId, DateTime sinceUtc) =>
        await _dbSet.CountAsync(s =>
            s.SwiperId == userId
            && s.CreatedAt >= sinceUtc
            && (s.Action == SwipeAction.Like || s.Action == SwipeAction.SuperLike));

    public async Task<int> CountSuperLikesSinceAsync(Guid userId, DateTime sinceUtc) =>
        await _dbSet.CountAsync(s =>
            s.SwiperId == userId
            && s.CreatedAt >= sinceUtc
            && s.Action == SwipeAction.SuperLike);
}
