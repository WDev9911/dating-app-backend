using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.Application.DTOs.Review;
using SameMess.Application.Interfaces.Services;

namespace SameMess.API.Controllers;

[Authorize]
[Route("api/reviews")]
public class ReviewsController : ApiControllerBase
{
    private readonly IReviewService _service;

    public ReviewsController(IReviewService service) => _service = service;

    /// <summary>Buổi hẹn đã diễn ra mà tôi chưa đánh giá (để hiện nút/form đánh giá).</summary>
    [HttpGet("pending")]
    public async Task<IActionResult> GetPending()
        => Ok(await _service.GetPendingAsync(CurrentUserId));

    /// <summary>Gửi đánh giá đối phương sau buổi hẹn (1..5 sao).</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDateReviewDto dto)
        => Ok(await _service.CreateAsync(CurrentUserId, dto));
}
