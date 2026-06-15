using SameMess.Application.DTOs.Admin;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Interfaces.Repositories;

namespace SameMess.Application.Services;

public class AdminDashboardService : IAdminDashboardService
{
    private readonly IAdminStatsRepository _statsRepository;

    public AdminDashboardService(IAdminStatsRepository statsRepository) => _statsRepository = statsRepository;

    public async Task<DashboardStatsDto> GetDashboardAsync()
    {
        var s = await _statsRepository.GetDashboardAsync();
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
