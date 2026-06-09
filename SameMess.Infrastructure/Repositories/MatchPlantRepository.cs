using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class MatchPlantRepository : BaseRepository<MatchPlant>, IMatchPlantRepository
{
    public MatchPlantRepository(AppDbContext context) : base(context) { }

    public async Task<MatchPlant?> GetByMatchIdAsync(Guid matchId) =>
        await _dbSet.FirstOrDefaultAsync(p => p.MatchId == matchId);
}
