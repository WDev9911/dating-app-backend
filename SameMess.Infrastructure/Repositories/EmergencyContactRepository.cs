using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class EmergencyContactRepository : BaseRepository<EmergencyContact>, IEmergencyContactRepository
{
    public EmergencyContactRepository(AppDbContext context) : base(context) { }

    public async Task<List<EmergencyContact>> GetByUserIdAsync(Guid userId) =>
        await _dbSet.Where(c => c.UserId == userId).OrderBy(c => c.CreatedAt).ToListAsync();

    public async Task ReplaceForUserAsync(Guid userId, IEnumerable<EmergencyContact> contacts)
    {
        var existing = await _dbSet.Where(c => c.UserId == userId).ToListAsync();
        _dbSet.RemoveRange(existing);
        await _dbSet.AddRangeAsync(contacts);
    }
}
