using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.Application.DTOs.Admin;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Enums;

namespace SameMess.API.Controllers;

[Authorize(Roles = UserRole.Admin)]
[Route("api/admin")]
public class AdminDashboardController : ApiControllerBase
{
    private readonly IAdminDashboardService _dashboardService;

    public AdminDashboardController(IAdminDashboardService dashboardService)
        => _dashboardService = dashboardService;

    /// <summary>Số liệu tổng quan dashboard.</summary>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(DashboardStatsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboard()
        => Ok(await _dashboardService.GetDashboardAsync());

    /// <summary>Dữ liệu biểu đồ theo ngày. type: signups | matches | revenue.</summary>
    [HttpGet("charts")]
    [ProducesResponseType(typeof(List<ChartPointDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCharts(
        [FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] string? type)
        => Ok(await _dashboardService.GetChartsAsync(from, to, type));
}
