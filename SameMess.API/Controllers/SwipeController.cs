using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.API.Extensions;
using SameMess.Application.DTOs.Matching;
using SameMess.Application.Interfaces.Services;

namespace SameMess.API.Controllers;

[Authorize]
[Route("api/swipes")]
public class SwipeController : ApiControllerBase
{
    private readonly ISwipeService _swipeService;
    private readonly IValidator<SwipeRequestDto> _swipeValidator;

    public SwipeController(ISwipeService swipeService, IValidator<SwipeRequestDto> swipeValidator)
    {
        _swipeService = swipeService;
        _swipeValidator = swipeValidator;
    }

    /// <summary>
    /// Swipe một người (Like / Pass / SuperLike). Nếu đối phương đã Like mình từ trước,
    /// một match sẽ được tạo và trả về isMatch = true kèm matchId.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(SwipeResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Swipe([FromBody] SwipeRequestDto dto)
    {
        var validation = await _swipeValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return ValidationProblem(validation.ToValidationProblemDetails());

        return Ok(await _swipeService.SwipeAsync(CurrentUserId, dto));
    }

    /// <summary>Những người đã Like (thường) tôi mà tôi chưa swipe lại.</summary>
    [HttpGet("liked-me")]
    [ProducesResponseType(typeof(List<LikedMeProfileDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWhoLikedMe()
        => Ok(await _swipeService.GetWhoLikedMeAsync(CurrentUserId));

    /// <summary>Mục riêng: những người đã SuperLike tôi mà tôi chưa phản hồi (Like lại → match, Pass → từ chối).</summary>
    [HttpGet("superliked-me")]
    [ProducesResponseType(typeof(List<LikedMeProfileDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWhoSuperLikedMe()
        => Ok(await _swipeService.GetWhoSuperLikedMeAsync(CurrentUserId));

    /// <summary>Hoàn tác lần swipe gần nhất (người đó sẽ quay lại feed).</summary>
    [HttpPost("undo")]
    [ProducesResponseType(typeof(UndoResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UndoLastSwipe()
        => Ok(await _swipeService.UndoLastSwipeAsync(CurrentUserId));
}
