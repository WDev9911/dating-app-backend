using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class DateReviewRepository : BaseRepository<DateReview>, IDateReviewRepository
{
    public DateReviewRepository(AppDbContext context) : base(context) { }

    public async Task<bool> ExistsAsync(Guid datePassOrderId, Guid reviewerId) =>
        await _dbSet.AnyAsync(r => r.DatePassOrderId == datePassOrderId && r.ReviewerId == reviewerId);

    public async Task<List<DateReview>> GetForRevieweeAsync(Guid revieweeId) =>
        await _dbSet.AsNoTracking()
            .Where(r => r.RevieweeId == revieweeId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task<(int Count, double Average)> GetSummaryAsync(Guid revieweeId)
    {
        var rows = await _dbSet.AsNoTracking()
            .Where(r => r.RevieweeId == revieweeId)
            .Select(r => r.Rating)
            .ToListAsync();
        if (rows.Count == 0) return (0, 0);
        return (rows.Count, rows.Average());
    }

    public async Task<HashSet<Guid>> GetReviewedOrderIdsAsync(Guid reviewerId, IEnumerable<Guid> orderIds)
    {
        var ids = orderIds.ToList();
        var reviewed = await _dbSet.AsNoTracking()
            .Where(r => r.ReviewerId == reviewerId && ids.Contains(r.DatePassOrderId))
            .Select(r => r.DatePassOrderId)
            .ToListAsync();
        return reviewed.ToHashSet();
    }
}
