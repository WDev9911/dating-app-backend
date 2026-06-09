using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class UserInventoryRepository : BaseRepository<UserInventory>, IUserInventoryRepository
{
    public UserInventoryRepository(AppDbContext context) : base(context) { }

    public async Task<List<UserInventory>> GetByUserAsync(Guid userId) =>
        await _dbSet.Where(i => i.UserId == userId).ToListAsync();

    public async Task<UserInventory?> GetAsync(Guid userId, string materialType) =>
        await _dbSet.FirstOrDefaultAsync(i => i.UserId == userId && i.MaterialType == materialType);
}
