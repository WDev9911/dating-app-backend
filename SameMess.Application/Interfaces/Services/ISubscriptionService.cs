using SameMess.Application.Billing;
using SameMess.Application.DTOs.Billing;

namespace SameMess.Application.Interfaces.Services;

public interface ISubscriptionService
{
    Task<List<PlanDto>> GetPlansAsync();
    Task<SubscriptionDto> GetMySubscriptionAsync(Guid userId);

    /// <summary>Quyền lợi hiện hành của user (cho các chỗ gate quyền dùng).</summary>
    Task<PlanEntitlements> GetEntitlementsAsync(Guid userId);

    /// <summary>Tạo đơn mua gói + trả URL thanh toán VNPay.</summary>
    Task<CreateOrderResultDto> CreateOrderAsync(Guid userId, string planCode, string clientIp);

    /// <summary>Xử lý Return (browser quay về) — chỉ verify để hiển thị, KHÔNG kích hoạt.</summary>
    Task<PaymentCallbackResult> HandleReturnAsync(IDictionary<string, string> query);

    /// <summary>Xử lý IPN (server→server) — xác nhận thẩm quyền: đánh dấu Paid + kích hoạt gói.</summary>
    Task<(string RspCode, string Message)> HandleIpnAsync(IDictionary<string, string> query);

    /// <summary>[DEV] Giả lập thanh toán thành công cho một đơn (test khi IPN không tới được localhost).</summary>
    Task<SubscriptionDto> MockConfirmAsync(Guid userId, string txnRef);

    /// <summary>Tạo đơn mua gói qua PayOS + trả về link/QR VietQR.</summary>
    Task<PayOsCreateResultDto> CreatePayOsOrderAsync(Guid userId, string planCode);

    /// <summary>Webhook PayOS đã verify → nếu là đơn mua gói thì đánh dấu Paid + kích hoạt (idempotent). Trả true nếu khớp đơn.</summary>
    Task<bool> TryHandlePayOsWebhookAsync(PayOsWebhookResult webhook);
}
