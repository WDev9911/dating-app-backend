using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class UserTaskProgressRepository : BaseRepository<UserTaskProgress>, IUserTaskProgressRepository
{
    public UserTaskProgressRepository(AppDbContext context) : base(context) { }

    public async Task<UserTaskProgress?> GetAsync(Guid userId, string taskCode, string periodKey) =>
        await _dbSet.FirstOrDefaultAsync(p =>
            p.UserId == userId && p.TaskCode == taskCode && p.PeriodKey == periodKey);

    public async Task<List<UserTaskProgress>> GetByUserAndKeysAsync(Guid userId, IEnumerable<string> periodKeys)
    {
        var keys = periodKeys.ToList();
        return await _dbSet
            .Where(p => p.UserId == userId && keys.Contains(p.PeriodKey))
            .ToListAsync();
    }
}
