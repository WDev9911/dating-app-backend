using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class PlanRepository : BaseRepository<Plan>, IPlanRepository
{
    public PlanRepository(AppDbContext context) : base(context) { }

    public async Task<List<Plan>> GetActiveAsync() =>
        await _dbSet.Where(p => p.IsActive).OrderBy(p => p.PriceVnd).ToListAsync();

    public async Task<Plan?> GetByCodeAsync(string code) =>
        await _dbSet.FirstOrDefaultAsync(p => p.Code == code);
}
