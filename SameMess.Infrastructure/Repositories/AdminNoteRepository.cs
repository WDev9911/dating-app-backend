using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class AdminNoteRepository : BaseRepository<AdminNote>, IAdminNoteRepository
{
    public AdminNoteRepository(AppDbContext context) : base(context) { }

    public async Task<List<AdminNote>> GetByUserIdAsync(Guid userId) =>
        await _dbSet.Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
}
