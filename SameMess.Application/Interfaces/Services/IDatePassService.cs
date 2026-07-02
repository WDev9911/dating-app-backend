using SameMess.Application.DTOs.Billing;
using SameMess.Application.DTOs.DatePass;

namespace SameMess.Application.Interfaces.Services;

public interface IDatePassService
{
    Task<List<VenueComboDto>> GetCombosAsync();
    Task<List<EligibleMatchDto>> GetEligibleMatchesAsync(Guid userId);
    Task<DatePassOrderDto> CreateOrderAsync(Guid userId, CreateDatePassOrderDto dto);
    Task<DatePassOrderDto> ConfirmAsync(Guid userId, Guid orderId);
    Task<DatePassOrderDto> RedeemAsync(Guid userId, Guid orderId);
    Task<List<DatePassOrderDto>> GetMyOrdersAsync(Guid userId);
    Task<DatePassRevenueDto> GetRevenueAsync();

    /// <summary>Tạo đơn ưu đãi + link thanh toán PayOS thật (VietQR).</summary>
    Task<PayOsCreateResultDto> CreatePayOsOrderAsync(Guid userId, CreateDatePassOrderDto dto);

    /// <summary>Webhook PayOS đã verify → nếu là đơn ưu đãi thì đánh dấu Paid + gửi voucher. Trả true nếu khớp đơn.</summary>
    Task<bool> TryHandlePayOsWebhookAsync(PayOsWebhookResult webhook);

    /// <summary>Hỏi PayOS trạng thái đơn theo orderCode rồi chốt Paid + gửi voucher (fallback khi webhook không tới). Trả true nếu là đơn ưu đãi.</summary>
    Task<bool> VerifyPayOsPaymentAsync(long orderCode);

    /// <summary>[Công khai] Thông tin voucher để quán quét QR xem (không cần đăng nhập).</summary>
    Task<VoucherPublicDto> GetVoucherAsync(Guid orderId);

    /// <summary>[Công khai] Quán xác nhận đã sử dụng voucher (không cần đăng nhập).</summary>
    Task<VoucherPublicDto> RedeemVoucherPublicAsync(Guid orderId);

    // Admin
    Task<List<VenueComboDto>> AdminListCombosAsync();
    Task<VenueComboDto> AdminCreateComboAsync(ComboPayloadDto dto);
    Task AdminDeleteComboAsync(Guid id);
}
