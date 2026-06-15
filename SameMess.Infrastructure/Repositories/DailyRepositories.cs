using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class UserXpRepository : BaseRepository<UserXp>, IUserXpRepository
{
    public UserXpRepository(AppDbContext context) : base(context) { }

    public async Task<UserXp?> GetByUserAsync(Guid userId) =>
        await _dbSet.FirstOrDefaultAsync(x => x.UserId == userId);
}

public class DailyQuestCompletionRepository : BaseRepository<DailyQuestCompletion>, IDailyQuestCompletionRepository
{
    public DailyQuestCompletionRepository(AppDbContext context) : base(context) { }

    public async Task<List<DailyQuestCompletion>> GetByUserAndPeriodAsync(Guid userId, string periodKey) =>
        await _dbSet.Where(c => c.UserId == userId && c.PeriodKey == periodKey).ToListAsync();
}
