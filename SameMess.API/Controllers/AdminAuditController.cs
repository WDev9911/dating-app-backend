using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Enums;

namespace SameMess.API.Controllers;

[Authorize(Roles = UserRole.Admin)]
[Route("api/admin/audit")]
public class AdminAuditController : ApiControllerBase
{
    private readonly IAuditService _auditService;

    public AdminAuditController(IAuditService auditService) => _auditService = auditService;

    /// <summary>Nhật ký hành động admin (phân trang, lọc theo action + adminId).</summary>
    [HttpGet]
    public async Task<IActionResult> GetLogs(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? action = null, [FromQuery] Guid? adminId = null)
        => Ok(await _auditService.GetLogsAsync(action, adminId, Math.Max(1, page), Math.Clamp(pageSize, 1, 100)));
}
