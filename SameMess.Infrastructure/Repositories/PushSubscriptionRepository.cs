using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class PushSubscriptionRepository : BaseRepository<PushSubscription>, IPushSubscriptionRepository
{
    public PushSubscriptionRepository(AppDbContext context) : base(context) { }

    public async Task<List<PushSubscription>> GetByUserAsync(Guid userId) =>
        await _dbSet.Where(s => s.UserId == userId).ToListAsync();

    public async Task<PushSubscription?> GetByEndpointAsync(Guid userId, string endpoint) =>
        await _dbSet.FirstOrDefaultAsync(s => s.UserId == userId && s.Endpoint == endpoint);
}
