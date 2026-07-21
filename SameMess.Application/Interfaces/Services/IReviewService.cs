using SameMess.Application.DTOs.Review;

namespace SameMess.Application.Interfaces.Services;

public interface IReviewService
{
    /// <summary>Tạo đánh giá đối phương sau buổi hẹn (voucher đã Redeemed).</summary>
    Task<DateReviewDto> CreateAsync(Guid reviewerId, CreateDateReviewDto dto);

    /// <summary>Các buổi hẹn đã diễn ra mà user chưa đánh giá (để hiện form/nút).</summary>
    Task<List<PendingReviewDto>> GetPendingAsync(Guid userId);

    /// <summary>Khối review gắn vào hồ sơ người khác — danh sách chỉ mở cho Gold, điểm TB hiện cho mọi người.</summary>
    Task<ProfileReviewsDto> GetProfileReviewsAsync(Guid viewerId, Guid targetUserId);
}
