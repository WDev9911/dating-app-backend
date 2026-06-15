using SameMess.Domain.Models;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IAdminStatsRepository
{
    Task<DashboardSnapshot> GetDashboardAsync();

    /// <summary>Chuỗi thời gian theo ngày cho biểu đồ. type: "signups" | "matches" | "revenue".</summary>
    Task<List<DailyCount>> GetDailySeriesAsync(string type, DateTime fromUtc, DateTime toUtc);
}
