namespace SameMess.Application.DTOs.Review;

/// <summary>Tạo đánh giá sau buổi hẹn (đánh giá đối phương).</summary>
public class CreateDateReviewDto
{
    public Guid DatePassOrderId { get; set; }
    public int Rating { get; set; }         // 1..5
    public string? Comment { get; set; }
}

/// <summary>Một đánh giá hiển thị trên hồ sơ (kèm thông tin người viết).</summary>
public class DateReviewDto
{
    public Guid Id { get; set; }
    public Guid ReviewerId { get; set; }
    public string ReviewerName { get; set; } = null!;
    public string? ReviewerAvatarUrl { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>Buổi hẹn đã diễn ra (voucher Redeemed) mà user CHƯA đánh giá — để hiện nút "Đánh giá".</summary>
public class PendingReviewDto
{
    public Guid DatePassOrderId { get; set; }
    public Guid PartnerId { get; set; }
    public string PartnerName { get; set; } = null!;
    public string? PartnerAvatarUrl { get; set; }
    public string VenueName { get; set; } = null!;
    public string ComboTitle { get; set; } = null!;
    public DateTime? RedeemedAt { get; set; }
}

/// <summary>Khối đánh giá gắn vào hồ sơ: điểm trung bình + danh sách (danh sách chỉ mở cho Gold).</summary>
public class ProfileReviewsDto
{
    public double RatingAvg { get; set; }
    public int RatingCount { get; set; }
    public bool Locked { get; set; }                      // có review nhưng người xem không phải Gold
    public List<DateReviewDto> Reviews { get; set; } = new();
}
