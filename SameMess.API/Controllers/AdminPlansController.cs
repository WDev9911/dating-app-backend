using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.Application.DTOs.Admin;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Enums;

namespace SameMess.API.Controllers;

[Authorize(Roles = UserRole.Admin)]
[Route("api/admin/plans")]
public class AdminPlansController : ApiControllerBase
{
    private readonly IAdminPlanService _service;

    public AdminPlansController(IAdminPlanService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> List() => Ok(await _service.ListAsync());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PlanPayloadDto dto)
        => Ok(await _service.CreateAsync(CurrentUserId, dto));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] PlanPayloadDto dto)
        => Ok(await _service.UpdateAsync(CurrentUserId, id, dto));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(CurrentUserId, id);
        return NoContent();
    }

    /// <summary>Danh sách người đăng ký thuê bao. ?planId=&status=active.</summary>
    [HttpGet("~/api/admin/subscribers")]
    public async Task<IActionResult> Subscribers([FromQuery] string? planId, [FromQuery] string? status)
        => Ok(await _service.GetSubscribersAsync(planId, status));
}
