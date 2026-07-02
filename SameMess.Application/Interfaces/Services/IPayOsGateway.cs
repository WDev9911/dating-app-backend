using SameMess.Application.DTOs.Billing;

namespace SameMess.Application.Interfaces.Services;

/// <summary>Cổng PayOS: tạo link thanh toán (VietQR) + verify webhook.</summary>
public interface IPayOsGateway
{
    /// <summary>Gốc URL frontend (dựng link trang voucher công khai + return/cancel Date Pass).</summary>
    string FrontendBaseUrl { get; }

    /// <summary>
    /// Tạo payment link/QR. description ≤ 25 ký tự (giới hạn PayOS).
    /// returnUrl/cancelUrl: nếu null dùng mặc định trong settings (luồng Premium);
    /// truyền riêng cho luồng khác (Date Pass) để quay về đúng trang.
    /// </summary>
    Task<PayOsCreateResultDto> CreatePaymentAsync(
        long orderCode, int amountVnd, string description,
        string? returnUrl = null, string? cancelUrl = null);

    /// <summary>Verify chữ ký + parse dữ liệu từ body webhook thô (JSON).</summary>
    PayOsWebhookResult VerifyWebhook(string rawJsonBody);

    /// <summary>Hỏi trực tiếp PayOS trạng thái đơn theo orderCode (chốt Paid khi user quay về, không cần webhook).</summary>
    Task<PayOsStatusResult> GetPaymentStatusAsync(long orderCode);
}
