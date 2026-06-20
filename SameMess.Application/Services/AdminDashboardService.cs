using SameMess.Application.DTOs.Admin;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Enums;
using SameMess.Domain.Interfaces.Repositories;

namespace SameMess.Application.Services;

public class AdminDashboardService : IAdminDashboardService
{
    private readonly IAdminStatsRepository _statsRepository;
    private readonly IDatePassOrderRepository _datePassRepository;

    public AdminDashboardService(IAdminStatsRepository statsRepository, IDatePassOrderRepository datePassRepository)
    {
        _statsRepository = statsRepository;
        _datePassRepository = datePassRepository;
    }

    public async Task<DashboardStatsDto> GetDashboardAsync()
    {
        var s = await _statsRepository.GetDashboardAsync();

        // Doanh thu combo (Date Pass) trên đơn đã thanh toán/đã dùng
        var orders = await _datePassRepository.GetAllOrdersAsync();
        var paid = orders.Where(o => o.Status == DatePassStatus.Paid || o.Status == DatePassStatus.Redeemed).ToList();
        long voucherGmv = paid.Sum(o => (long)o.AmountVnd);
        long voucherCommission = paid.Sum(o => (long)o.CommissionVnd);

        return new DashboardStatsDto
        {
            TotalUsers = s.TotalUsers,
            ActiveUsers = s.ActiveUsers,
            BannedUsers = s.BannedUsers,
            PendingVerifications = s.PendingVerifications,
            NewUsersToday = s.NewUsersToday,
            NewUsers7Days = s.NewUsers7Days,
            TotalMatches = s.TotalMatches,
            PendingReports = s.PendingReports,
            ActiveSubscriptions = s.ActiveSubscriptions,
            RevenueVnd = s.RevenueVnd,
            VoucherOrders = paid.Count,
            VoucherGmvVnd = voucherGmv,
            VoucherCommissionVnd = voucherCommission,
            TotalRevenueVnd = s.RevenueVnd + voucherCommission,
        };
    }

    public async Task<List<ChartPointDto>> GetChartsAsync(DateTime? from, DateTime? to, string? type)
    {
        var toUtc = (to ?? DateTime.UtcNow).Date.AddDays(1);          // bao trọn ngày cuối
        var fromUtc = (from ?? DateTime.UtcNow.AddDays(-30)).Date;

        var series = await _statsRepository.GetDailySeriesAsync(type ?? "signups", fromUtc, toUtc);
        return series.Select(d => new ChartPointDto { Date = d.Date, Value = d.Value }).ToList();
    }
}
