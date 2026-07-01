using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.Application.Interfaces.Services;

namespace SameMess.API.Controllers;

[Route("api/payments/payos")]
public class PayOsController : ApiControllerBase
{
    private readonly ISubscriptionService _subscriptionService;

    public PayOsController(ISubscriptionService subscriptionService)
        => _subscriptionService = subscriptionService;

    public record CreatePayOsDto(string PlanCode);

    /// <summary>Tạo đơn mua gói qua PayOS → trả checkoutUrl + QR để frontend hiển thị/chuyển hướng.</summary>
    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreatePayOsDto dto)
    {
        var result = await _subscriptionService.CreatePayOsOrderAsync(CurrentUserId, dto.PlanCode);
        return Ok(result);
    }

    /// <summary>
    /// Webhook PayOS (server→server). PayOS POST JSON kèm chữ ký; verify + kích hoạt gói (idempotent).
    /// Luôn trả 200 để PayOS không retry vô hạn; chỉ báo lỗi khi chữ ký sai.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook()
    {
        using var reader = new StreamReader(Request.Body);
        var body = await reader.ReadToEndAsync();

        var ok = await _subscriptionService.HandlePayOsWebhookAsync(body);
        if (!ok) return BadRequest(new { success = false, message = "Invalid signature" });
        return Ok(new { success = true });
    }
}
