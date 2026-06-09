namespace SameMess.Domain.Enums;

public static class PaymentStatus
{
    public const string Pending = "Pending"; // vừa tạo đơn, chờ thanh toán
    public const string Paid = "Paid";       // VNPay xác nhận thành công
    public const string Failed = "Failed";   // thanh toán thất bại / hủy
}
