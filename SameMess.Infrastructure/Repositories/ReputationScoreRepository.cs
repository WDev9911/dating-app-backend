using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class ReputationScoreRepository : BaseRepository<ReputationScore>, IReputationScoreRepository
{
    public ReputationScoreRepository(AppDbContext context) : base(context) { }

    public async Task<ReputationScore?> GetByUserAsync(Guid userId) =>
        await _dbSet.FirstOrDefaultAsync(s => s.UserId == userId);

    public async Task<List<ReputationScore>> GetByUserIdsAsync(IEnumerable<Guid> userIds)
    {
        var ids = userIds.ToList();
        return await _dbSet.Where(s => ids.Contains(s.UserId)).ToListAsync();
    }
}
