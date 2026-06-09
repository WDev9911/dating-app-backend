using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class ReputationEventRepository : BaseRepository<ReputationEvent>, IReputationEventRepository
{
    public ReputationEventRepository(AppDbContext context) : base(context) { }

    public async Task<List<ReputationEvent>> GetByUserAsync(Guid userId) =>
        await _dbSet
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();

    public async Task<bool> ExistsAsync(Guid userId, string type) =>
        await _dbSet.AnyAsync(e => e.UserId == userId && e.Type == type);
}
