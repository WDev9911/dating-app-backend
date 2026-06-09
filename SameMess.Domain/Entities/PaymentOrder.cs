namespace SameMess.Domain.Entities;

/// <summary>
/// Đơn thanh toán một gói qua VNPay. TxnRef là mã đơn gửi sang VNPay (vnp_TxnRef, duy nhất).
/// Kích hoạt thuê bao CHỈ khi đơn chuyển sang Paid (qua IPN — xác nhận server-to-server).
/// </summary>
public class PaymentOrder
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string TxnRef { get; set; } = null!;   // vnp_TxnRef
    public string PlanCode { get; set; } = null!;  // gói đang mua
    public int AmountVnd { get; set; }
    public string Status { get; set; } = null!;    // PaymentStatus
    public string? VnpTransactionNo { get; set; }  // mã giao dịch VNPay trả về
    public string? VnpResponseCode { get; set; }   // "00" = thành công
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
}
