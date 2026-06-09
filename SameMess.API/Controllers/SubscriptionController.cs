using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.Application.DTOs.Billing;
using SameMess.Application.Interfaces.Services;

namespace SameMess.API.Controllers;

[Authorize]
[Route("api")]
public class SubscriptionController : ApiControllerBase
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionController(ISubscriptionService subscriptionService)
        => _subscriptionService = subscriptionService;

    /// <summary>Danh sách gói (giá, thời hạn).</summary>
    [HttpGet("plans")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<PlanDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlans()
        => Ok(await _subscriptionService.GetPlansAsync());

    /// <summary>Thuê bao + quyền lợi hiện tại của tôi.</summary>
    [HttpGet("subscription/me")]
    [ProducesResponseType(typeof(SubscriptionDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMySubscription()
        => Ok(await _subscriptionService.GetMySubscriptionAsync(CurrentUserId));

    /// <summary>Tạo đơn mua gói (Plus/Gold) → trả URL thanh toán VNPay để redirect.</summary>
    [HttpPost("subscription/order")]
    [ProducesResponseType(typeof(CreateOrderResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto dto)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        return Ok(await _subscriptionService.CreateOrderAsync(CurrentUserId, dto.PlanCode, ip));
    }

    /// <summary>[DEV] Giả lập thanh toán thành công (test khi IPN không tới được localhost).</summary>
    [HttpPost("subscription/mock-confirm/{txnRef}")]
    [ProducesResponseType(typeof(SubscriptionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MockConfirm(string txnRef)
        => Ok(await _subscriptionService.MockConfirmAsync(CurrentUserId, txnRef));
}
