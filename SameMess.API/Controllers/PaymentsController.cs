using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.Application.Interfaces.Services;

namespace SameMess.API.Controllers;

[AllowAnonymous] // VNPay gọi về (browser/redirect & server→server) — không có JWT; định danh qua TxnRef
[Route("api/payments/vnpay")]
public class PaymentsController : ApiControllerBase
{
    private readonly ISubscriptionService _subscriptionService;

    public PaymentsController(ISubscriptionService subscriptionService)
        => _subscriptionService = subscriptionService;

    /// <summary>VNPay redirect browser về đây sau thanh toán. CHỈ hiển thị kết quả (không kích hoạt).</summary>
    [HttpGet("return")]
    public async Task<IActionResult> Return()
    {
        var query = QueryToDict();
        var result = await _subscriptionService.HandleReturnAsync(query);
        return Ok(new
        {
            success = result.IsSuccess,
            signatureValid = result.SignatureValid,
            responseCode = result.ResponseCode,
            txnRef = result.TxnRef,
            message = result.IsSuccess
                ? "Thanh toán thành công. Gói đã được kích hoạt."
                : "Thanh toán không thành công hoặc bị hủy.",
        });
    }

    /// <summary>IPN VNPay (server→server) — xác nhận thẩm quyền: đánh dấu Paid + kích hoạt gói.</summary>
    [HttpGet("ipn")]
    public async Task<IActionResult> Ipn()
    {
        var query = QueryToDict();
        var (rspCode, message) = await _subscriptionService.HandleIpnAsync(query);
        return Ok(new { RspCode = rspCode, Message = message });
    }

    private Dictionary<string, string> QueryToDict() =>
        HttpContext.Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString());
}
