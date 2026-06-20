using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IDatePassOrderRepository : IBaseRepository<DatePassOrder>
{
    /// <summary>Voucher đang hiệu lực (Pending/Paid) của cặp cho đúng loại combo — để chặn mua trùng.</summary>
    Task<DatePassOrder?> GetActiveForCoupleComboAsync(Guid matchId, Guid comboId);

    /// <summary>Tất cả đơn của các match mà user thuộc về.</summary>
    Task<List<DatePassOrder>> GetForMatchesAsync(IEnumerable<Guid> matchIds);

    /// <summary>Tìm theo mã voucher.</summary>
    Task<DatePassOrder?> GetByVoucherCodeAsync(string voucherCode);

    /// <summary>Toàn bộ đơn (cho dashboard doanh thu).</summary>
    Task<List<DatePassOrder>> GetAllOrdersAsync();
}
