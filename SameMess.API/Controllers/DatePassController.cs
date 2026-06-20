using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.Application.DTOs.DatePass;
using SameMess.Application.Interfaces.Services;

namespace SameMess.API.Controllers;

[Authorize]
[Route("api/datepass")]
public class DatePassController : ApiControllerBase
{
    private readonly IDatePassService _service;

    public DatePassController(IDatePassService service) => _service = service;

    /// <summary>Danh sách combo ưu đãi của các quán (ai cũng xem được).</summary>
    [HttpGet("combos")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCombos() => Ok(await _service.GetCombosAsync());

    /// <summary>Các cặp match đủ điều kiện đặt combo (cây ≥ Cấp 4).</summary>
    [HttpGet("eligible-matches")]
    public async Task<IActionResult> GetEligibleMatches() => Ok(await _service.GetEligibleMatchesAsync(CurrentUserId));

    /// <summary>Tạo đơn (chưa thanh toán).</summary>
    [HttpPost("order")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateDatePassOrderDto dto)
        => Ok(await _service.CreateOrderAsync(CurrentUserId, dto));

    /// <summary>Thanh toán (mock) → phát voucher + gửi email.</summary>
    [HttpPost("order/{orderId:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid orderId)
        => Ok(await _service.ConfirmAsync(CurrentUserId, orderId));

    /// <summary>Quán quét QR xác nhận đã sử dụng.</summary>
    [HttpPost("order/{orderId:guid}/redeem")]
    public async Task<IActionResult> Redeem(Guid orderId)
        => Ok(await _service.RedeemAsync(CurrentUserId, orderId));

    /// <summary>Voucher của tôi (theo các cặp match của tôi).</summary>
    [HttpGet("my")]
    public async Task<IActionResult> GetMine() => Ok(await _service.GetMyOrdersAsync(CurrentUserId));

    /// <summary>Doanh thu (dashboard demo).</summary>
    [HttpGet("revenue")]
    public async Task<IActionResult> GetRevenue() => Ok(await _service.GetRevenueAsync());
}
