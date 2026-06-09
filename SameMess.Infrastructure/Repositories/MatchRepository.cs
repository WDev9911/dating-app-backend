using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class MatchRepository : BaseRepository<Match>, IMatchRepository
{
    public MatchRepository(AppDbContext context) : base(context) { }

    public async Task<Match?> GetByPairAsync(Guid userAId, Guid userBId) =>
        await _dbSet.FirstOrDefaultAsync(m => m.UserAId == userAId && m.UserBId == userBId);

    public async Task<List<Match>> GetActiveForUserAsync(Guid userId) =>
        await _dbSet
            .Where(m => m.IsActive && (m.UserAId == userId || m.UserBId == userId))
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

    public async Task<Match?> GetByIdForUserAsync(Guid matchId, Guid userId) =>
        await _dbSet.FirstOrDefaultAsync(m =>
            m.Id == matchId && (m.UserAId == userId || m.UserBId == userId));
}
