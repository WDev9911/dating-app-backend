using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.Application.DTOs.Gamification;
using SameMess.Application.Interfaces.Services;

namespace SameMess.API.Controllers;

[Authorize]
[Route("api")]
public class GamificationController : ApiControllerBase
{
    private readonly ITaskService _taskService;

    public GamificationController(ITaskService taskService)
        => _taskService = taskService;

    /// <summary>Nhiệm vụ + tiến độ kỳ hiện tại (mở danh sách cũng tính là check-in hôm nay).</summary>
    [HttpGet("tasks")]
    [ProducesResponseType(typeof(List<TaskDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTasks()
        => Ok(await _taskService.GetTasksAsync(CurrentUserId));

    /// <summary>Kho nguyên liệu của tôi (Water/Sun/Fertilizer).</summary>
    [HttpGet("inventory")]
    [ProducesResponseType(typeof(List<InventoryItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInventory()
        => Ok(await _taskService.GetInventoryAsync(CurrentUserId));
}
