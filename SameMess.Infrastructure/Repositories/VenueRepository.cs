using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class VenueRepository : BaseRepository<Venue>, IVenueRepository
{
    public VenueRepository(AppDbContext context) : base(context) { }

    public async Task<List<Venue>> GetActiveAsync(string? category)
    {
        var query = _dbSet.Where(v => v.IsActive);
        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(v => v.Category == category);
        return await query.ToListAsync();
    }

    public async Task<List<Venue>> GetAllAsync(bool includeInactive)
    {
        var query = includeInactive ? _dbSet : _dbSet.Where(v => v.IsActive);
        return await query.OrderBy(v => v.District).ThenBy(v => v.Name).ToListAsync();
    }
}
