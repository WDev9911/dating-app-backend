namespace SameMess.Application.DTOs.DatePass;

/// <summary>Combo của một quán để hiển thị ở trang ưu đãi (ai cũng xem được).</summary>
public class VenueComboDto
{
    public Guid Id { get; set; }
    public Guid VenueId { get; set; }
    public string VenueName { get; set; } = null!;
    public string Category { get; set; } = null!;
    public string? VenueAddress { get; set; }
    public string? VenueImageUrl { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int OriginalPriceVnd { get; set; }
    public int SalePriceVnd { get; set; }
    public int DiscountPercent { get; set; }
    public int CommissionPercent { get; set; }
}

/// <summary>Admin tạo/sửa combo cho một quán.</summary>
public class ComboPayloadDto
{
    public Guid VenueId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int OriginalPriceVnd { get; set; }
    public int SalePriceVnd { get; set; }
    public int CommissionPercent { get; set; } = 15;
    public bool IsActive { get; set; } = true;
}

/// <summary>Match đủ điều kiện đặt combo (cây ≥ Level 4).</summary>
public class EligibleMatchDto
{
    public Guid MatchId { get; set; }
    public string DisplayName { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public int Level { get; set; }
}

public class CreateDatePassOrderDto
{
    public Guid ComboId { get; set; }
    public Guid MatchId { get; set; }
    // Voucher luôn gửi tới email đăng ký của cả hai người — không nhận email từ client (chống lạm dụng).
}

public class DatePassOrderDto
{
    public Guid Id { get; set; }
    public Guid MatchId { get; set; }
    public string PartnerName { get; set; } = null!;
    public string VenueName { get; set; } = null!;
    public string ComboTitle { get; set; } = null!;
    public int AmountVnd { get; set; }
    public int CommissionVnd { get; set; }
    public string VoucherCode { get; set; } = null!;
    public string QrUrl { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string? Email { get; set; }
    public bool IsMine { get; set; }            // user hiện tại là người mua?
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RedeemedAt { get; set; }
}

/// <summary>
/// Thông tin voucher hiển thị ở TRANG CÔNG KHAI (quán quét QR mở ra) — không cần đăng nhập.
/// Chứa đủ dữ liệu để quán xác nhận buổi hẹn.
/// </summary>
public class VoucherPublicDto
{
    public Guid Id { get; set; }
    public string BuyerName { get; set; } = null!;
    public string PartnerName { get; set; } = null!;
    public string VenueName { get; set; } = null!;
    public string ComboTitle { get; set; } = null!;
    public int AmountVnd { get; set; }
    public string VoucherCode { get; set; } = null!;
    public string Status { get; set; } = null!;   // Pending / Paid / Redeemed / Cancelled
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RedeemedAt { get; set; }
    public bool IsExpired { get; set; }            // quá hạn dùng?
    public bool CanRedeem { get; set; }            // đủ điều kiện để quán bấm "đã sử dụng"?
}

/// <summary>Số liệu doanh thu (dashboard demo).</summary>
public class DatePassRevenueDto
{
    public int TotalOrders { get; set; }
    public int PaidOrders { get; set; }
    public int RedeemedOrders { get; set; }
    public long GmvVnd { get; set; }            // tổng tiền combo đã bán
    public long CommissionVnd { get; set; }     // tổng hoa hồng app thu
}
