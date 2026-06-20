using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.Application.Interfaces.Services;

namespace SameMess.API.Controllers;

[AllowAnonymous] // VNPay gọi về (browser/redirect & server→server) — không có JWT; định danh qua TxnRef
[Route("api/payments/vnpay")]
public class PaymentsController : ApiControllerBase
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly IConfiguration _config;

    public PaymentsController(ISubscriptionService subscriptionService, IConfiguration config)
    {
        _subscriptionService = subscriptionService;
        _config = config;
    }

    /// <summary>
    /// VNPay redirect browser về đây sau thanh toán. Xác minh + kích hoạt gói (nếu thành công),
    /// rồi CHUYỂN HƯỚNG người dùng về web app (trang Premium) kèm trạng thái.
    /// </summary>
    [HttpGet("return")]
    public async Task<IActionResult> Return()
    {
        var query = QueryToDict();
        var result = await _subscriptionService.HandleReturnAsync(query);

        var baseUrl = _config["VNPay:FrontendReturnUrl"]?.TrimEnd('/')
                      ?? "http://localhost:5173/premium";
        var status = result.IsSuccess ? "success" : "failed";
        var sep = baseUrl.Contains('?') ? "&" : "?";
        var redirectUrl = $"{baseUrl}{sep}payment={status}";
        return Redirect(redirectUrl);
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
