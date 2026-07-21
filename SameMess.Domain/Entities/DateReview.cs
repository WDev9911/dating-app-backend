namespace SameMess.Domain.Entities;

/// <summary>
/// Đánh giá sau buổi hẹn: sau khi voucher Date Pass được quán xác nhận đã dùng (Redeemed),
/// mỗi người trong cặp đánh giá ĐỐI PHƯƠNG (1 lần / đơn). Điểm sao gắn vào hồ sơ người được đánh giá.
/// </summary>
public class DateReview
{
    public Guid Id { get; set; }
    public Guid DatePassOrderId { get; set; }  // buổi hẹn (voucher) mà review này gắn vào
    public Guid MatchId { get; set; }          // cặp match (tiện thống kê)
    public Guid ReviewerId { get; set; }       // người viết đánh giá
    public Guid RevieweeId { get; set; }       // người được đánh giá (đối phương)

    public int Rating { get; set; }            // 1..5 sao
    public string? Comment { get; set; }       // nhận xét ngắn (tuỳ chọn)

    public DateTime CreatedAt { get; set; }
}
