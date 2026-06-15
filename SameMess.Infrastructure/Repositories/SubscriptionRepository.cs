using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class SubscriptionRepository : BaseRepository<Subscription>, ISubscriptionRepository
{
    public SubscriptionRepository(AppDbContext context) : base(context) { }

    public async Task<Subscription?> GetByUserAsync(Guid userId) =>
        await _dbSet.FirstOrDefaultAsync(s => s.UserId == userId);

    public async Task<List<Subscription>> GetSubscribersAsync(string? planCode, bool activeOnly)
    {
        var query = _dbSet.AsQueryable();
        if (!string.IsNullOrWhiteSpace(planCode))
            query = query.Where(s => s.PlanCode == planCode);
        if (activeOnly)
            query = query.Where(s => s.ExpiresAt > DateTime.UtcNow);
        return await query.OrderByDescending(s => s.UpdatedAt).ToListAsync();
    }
}
