using SameMess.Domain.Enums;

namespace SameMess.Domain.Entities;

/// <summary>
/// Đơn mua combo hẹn hò (Date Pass) — gắn với một CẶP (Match). Voucher dùng chung cho cả hai,
/// chỉ một người mua. Quán quét QR để xác nhận đã sử dụng.
/// </summary>
public class DatePassOrder
{
    public Guid Id { get; set; }
    public Guid MatchId { get; set; }       // voucher thuộc về cặp này
    public Guid BuyerId { get; set; }       // người đã mua
    public Guid VenueId { get; set; }
    public Guid ComboId { get; set; }

    // Snapshot để hiển thị/hoá đơn không phụ thuộc dữ liệu gốc đổi
    public string VenueName { get; set; } = null!;
    public string ComboTitle { get; set; } = null!;

    public int AmountVnd { get; set; }       // số tiền đã trả (SalePrice)
    public int CommissionVnd { get; set; }   // hoa hồng app thu
    public string VoucherCode { get; set; } = null!;
    public string? Email { get; set; }       // email nhận voucher

    public string Status { get; set; } = DatePassStatus.Pending;
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime? RedeemedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}
