using SameMess.Application.DTOs.Admin;

namespace SameMess.Application.Interfaces.Services;

public interface IAdminDashboardService
{
    Task<DashboardStatsDto> GetDashboardAsync();
    Task<List<ChartPointDto>> GetChartsAsync(DateTime? from, DateTime? to, string? type);
}
