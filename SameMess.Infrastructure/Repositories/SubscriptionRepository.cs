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
}
