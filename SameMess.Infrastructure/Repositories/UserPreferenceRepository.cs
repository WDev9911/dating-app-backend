using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class UserPreferenceRepository : BaseRepository<UserPreference>, IUserPreferenceRepository
{
    public UserPreferenceRepository(AppDbContext context) : base(context) { }

    public async Task<UserPreference?> GetByUserIdAsync(Guid userId) =>
        await _dbSet.FirstOrDefaultAsync(p => p.UserId == userId);
}
