using SameMess.Application.DTOs.Billing;

namespace SameMess.Application.Interfaces.Services;

/// <summary>Cổng PayOS: tạo link thanh toán (VietQR) + verify webhook.</summary>
public interface IPayOsGateway
{
    /// <summary>Tạo payment link/QR. description ≤ 25 ký tự (giới hạn PayOS).</summary>
    Task<PayOsCreateResultDto> CreatePaymentAsync(long orderCode, int amountVnd, string description);

    /// <summary>Verify chữ ký + parse dữ liệu từ body webhook thô (JSON).</summary>
    PayOsWebhookResult VerifyWebhook(string rawJsonBody);
}
