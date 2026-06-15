using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class PhotoRepository : BaseRepository<Photo>, IPhotoRepository
{
    public PhotoRepository(AppDbContext context) : base(context) { }

    public async Task<List<Photo>> GetByUserIdAsync(Guid userId) =>
        await _dbSet.Where(p => p.UserId == userId)
            .OrderBy(p => p.OrderIndex)
            .ToListAsync();

    public async Task<List<Photo>> GetByStatusAsync(string status) =>
        await _dbSet.Include(p => p.User).ThenInclude(u => u.Profile)
            .Where(p => p.Status == status)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
}
