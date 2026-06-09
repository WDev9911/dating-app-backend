using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.Application.DTOs.Safety;
using SameMess.Application.Interfaces.Services;

namespace SameMess.API.Controllers;

[Authorize]
[Route("api")]
public class BlockController : ApiControllerBase
{
    private readonly IBlockService _blockService;

    public BlockController(IBlockService blockService) => _blockService = blockService;

    /// <summary>Block một người: ẩn nhau khỏi feed (2 chiều) và hủy match nếu có.</summary>
    [HttpPost("users/{id:guid}/block")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Block(Guid id)
    {
        await _blockService.BlockAsync(CurrentUserId, id);
        return NoContent();
    }

    /// <summary>Bỏ block (không tự khôi phục match cũ).</summary>
    [HttpDelete("users/{id:guid}/block")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Unblock(Guid id)
    {
        await _blockService.UnblockAsync(CurrentUserId, id);
        return NoContent();
    }

    /// <summary>Danh sách những người tôi đã block.</summary>
    [HttpGet("blocks")]
    [ProducesResponseType(typeof(List<BlockedUserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyBlocks()
        => Ok(await _blockService.GetMyBlocksAsync(CurrentUserId));
}
