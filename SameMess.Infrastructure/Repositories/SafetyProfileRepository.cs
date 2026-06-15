using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class SafetyProfileRepository : BaseRepository<SafetyProfile>, ISafetyProfileRepository
{
    public SafetyProfileRepository(AppDbContext context) : base(context) { }

    public async Task<SafetyProfile?> GetByUserIdAsync(Guid userId) =>
        await _dbSet.FirstOrDefaultAsync(s => s.UserId == userId);
}
