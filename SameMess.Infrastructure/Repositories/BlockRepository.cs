using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class BlockRepository : BaseRepository<Block>, IBlockRepository
{
    public BlockRepository(AppDbContext context) : base(context) { }

    public async Task<Block?> GetAsync(Guid blockerId, Guid blockedId) =>
        await _dbSet.FirstOrDefaultAsync(b => b.BlockerId == blockerId && b.BlockedId == blockedId);

    public async Task<bool> ExistsBetweenAsync(Guid user1, Guid user2) =>
        await _dbSet.AnyAsync(b =>
            (b.BlockerId == user1 && b.BlockedId == user2)
            || (b.BlockerId == user2 && b.BlockedId == user1));

    public async Task<List<Guid>> GetRelatedUserIdsAsync(Guid userId)
    {
        var blockedByMe = _dbSet.Where(b => b.BlockerId == userId).Select(b => b.BlockedId);
        var blockedMe = _dbSet.Where(b => b.BlockedId == userId).Select(b => b.BlockerId);
        return await blockedByMe.Union(blockedMe).ToListAsync();
    }

    public async Task<List<Block>> GetByBlockerAsync(Guid blockerId) =>
        await _dbSet.Where(b => b.BlockerId == blockerId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
}
