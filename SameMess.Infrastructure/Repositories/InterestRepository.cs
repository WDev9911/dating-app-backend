using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class InterestRepository : BaseRepository<Interest>, IInterestRepository
{
    public InterestRepository(AppDbContext context) : base(context) { }

    public async Task<List<Interest>> GetActiveAsync() =>
        await _dbSet.Where(i => i.IsActive)
            .OrderBy(i => i.GroupName).ThenBy(i => i.Name)
            .ToListAsync();

    public async Task<List<Interest>> GetByIdsAsync(IReadOnlyCollection<Guid> ids) =>
        await _dbSet.Where(i => ids.Contains(i.Id)).ToListAsync();
}
