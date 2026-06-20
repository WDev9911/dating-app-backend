namespace SameMess.Domain.Enums;

/// <summary>Trạng thái đơn mua combo hẹn hò (Date Pass).</summary>
public static class DatePassStatus
{
    public const string Pending = "Pending";     // tạo đơn, chưa thanh toán
    public const string Paid = "Paid";           // đã thanh toán → voucher có hiệu lực
    public const string Redeemed = "Redeemed";   // quán đã quét → đã sử dụng
    public const string Cancelled = "Cancelled"; // huỷ / hết hạn
}
