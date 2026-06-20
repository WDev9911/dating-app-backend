using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class VenueComboRepository : BaseRepository<VenueCombo>, IVenueComboRepository
{
    public VenueComboRepository(AppDbContext context) : base(context) { }

    public async Task<List<VenueCombo>> GetActiveWithVenueAsync() =>
        await _dbSet.AsNoTracking()
            .Include(c => c.Venue)
            .Where(c => c.IsActive && c.Venue.IsActive)
            .OrderBy(c => c.Venue.Name).ThenBy(c => c.SalePriceVnd)
            .ToListAsync();

    public async Task<VenueCombo?> GetWithVenueAsync(Guid id) =>
        await _dbSet.Include(c => c.Venue).FirstOrDefaultAsync(c => c.Id == id);
}
