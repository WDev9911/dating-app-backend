using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Enums;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class DatePassOrderRepository : BaseRepository<DatePassOrder>, IDatePassOrderRepository
{
    public DatePassOrderRepository(AppDbContext context) : base(context) { }

    public async Task<DatePassOrder?> GetActiveForCoupleComboAsync(Guid matchId, Guid comboId) =>
        await _dbSet.FirstOrDefaultAsync(o =>
            o.MatchId == matchId && o.ComboId == comboId &&
            (o.Status == DatePassStatus.Pending || o.Status == DatePassStatus.Paid));

    public async Task<List<DatePassOrder>> GetForMatchesAsync(IEnumerable<Guid> matchIds)
    {
        var ids = matchIds.ToList();
        return await _dbSet.AsNoTracking()
            .Where(o => ids.Contains(o.MatchId))
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<DatePassOrder?> GetByVoucherCodeAsync(string voucherCode) =>
        await _dbSet.FirstOrDefaultAsync(o => o.VoucherCode == voucherCode);

    public async Task<DatePassOrder?> GetByPayOsOrderCodeAsync(long payOsOrderCode) =>
        await _dbSet.FirstOrDefaultAsync(o => o.PayOsOrderCode == payOsOrderCode);

    public async Task<List<DatePassOrder>> GetAllOrdersAsync() =>
        await _dbSet.AsNoTracking().OrderByDescending(o => o.CreatedAt).ToListAsync();
}
