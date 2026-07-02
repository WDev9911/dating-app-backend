using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.Application.Interfaces.Services;

namespace SameMess.API.Controllers;

[Route("api/payments/payos")]
public class PayOsController : ApiControllerBase
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly IDatePassService _datePassService;
    private readonly IPayOsGateway _payos;

    public PayOsController(
        ISubscriptionService subscriptionService,
        IDatePassService datePassService,
        IPayOsGateway payos)
    {
        _subscriptionService = subscriptionService;
        _datePassService = datePassService;
        _payos = payos;
    }

    public record CreatePayOsDto(string PlanCode);
    public record VerifyPayOsDto(long OrderCode);

    /// <summary>Tạo đơn mua gói qua PayOS → trả checkoutUrl + QR để frontend hiển thị/chuyển hướng.</summary>
    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreatePayOsDto dto)
    {
        var result = await _subscriptionService.CreatePayOsOrderAsync(CurrentUserId, dto.PlanCode);
        return Ok(result);
    }

    /// <summary>
    /// Chốt thanh toán khi user quay về từ PayOS: hỏi thẳng PayOS trạng thái đơn rồi kích hoạt.
    /// Fallback cho webhook (Render Free hay ngủ → webhook có thể trượt). Thử đơn mua gói trước, rồi ưu đãi.
    /// </summary>
    [Authorize]
    [HttpPost("verify")]
    public async Task<IActionResult> Verify([FromBody] VerifyPayOsDto dto)
    {
        var handled = await _subscriptionService.VerifyPayOsPaymentAsync(dto.OrderCode);
        if (!handled) await _datePassService.VerifyPayOsPaymentAsync(dto.OrderCode);
        return Ok(new { ok = true });
    }

    /// <summary>
    /// Webhook PayOS (server→server) dùng CHUNG cho mọi loại đơn (mua gói + ưu đãi Date Pass).
    /// Verify chữ ký 1 lần rồi định tuyến: thử đơn mua gói trước, nếu không khớp thì thử đơn ưu đãi.
    /// Luôn trả 200 để PayOS không retry vô hạn; chỉ báo lỗi khi chữ ký sai.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook()
    {
        using var reader = new StreamReader(Request.Body);
        var body = await reader.ReadToEndAsync();

        var v = _payos.VerifyWebhook(body);
        if (!v.SignatureValid)
            return BadRequest(new { success = false, message = "Invalid signature" });

        // Thử khớp đơn mua gói; nếu không phải thì thử đơn ưu đãi Date Pass.
        var handled = await _subscriptionService.TryHandlePayOsWebhookAsync(v);
        if (!handled)
            await _datePassService.TryHandlePayOsWebhookAsync(v);

        return Ok(new { success = true });
    }
}
