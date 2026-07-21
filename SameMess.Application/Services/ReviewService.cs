using SameMess.Application.DTOs.Review;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Entities;
using SameMess.Domain.Enums;
using SameMess.Domain.Exceptions;
using SameMess.Domain.Interfaces.Repositories;

namespace SameMess.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IDateReviewRepository _reviewRepository;
    private readonly IDatePassOrderRepository _orderRepository;
    private readonly IMatchService _matchService;
    private readonly IUserRepository _userRepository;
    private readonly ISubscriptionService _subscriptionService;

    public ReviewService(
        IDateReviewRepository reviewRepository,
        IDatePassOrderRepository orderRepository,
        IMatchService matchService,
        IUserRepository userRepository,
        ISubscriptionService subscriptionService)
    {
        _reviewRepository = reviewRepository;
        _orderRepository = orderRepository;
        _matchService = matchService;
        _userRepository = userRepository;
        _subscriptionService = subscriptionService;
    }

    public async Task<DateReviewDto> CreateAsync(Guid reviewerId, CreateDateReviewDto dto)
    {
        if (dto.Rating < 1 || dto.Rating > 5)
            throw new BadRequestException("Điểm đánh giá phải từ 1 đến 5 sao.");

        var order = await _orderRepository.GetByIdAsync(dto.DatePassOrderId)
            ?? throw new NotFoundException("Buổi hẹn", dto.DatePassOrderId);
        if (order.Status != DatePassStatus.Redeemed)
            throw new BadRequestException("Chỉ có thể đánh giá sau khi buổi hẹn đã được xác nhận tại quán.");

        // Người đánh giá phải thuộc cặp; đối phương = người còn lại
        Guid revieweeId;
        if (order.BuyerId == reviewerId)
            revieweeId = order.PartnerId ?? throw new BadRequestException("Không xác định được đối phương của buổi hẹn này.");
        else if (order.PartnerId == reviewerId)
            revieweeId = order.BuyerId;
        else
            throw new ForbiddenException("Bạn không thuộc buổi hẹn này.");

        if (await _reviewRepository.ExistsAsync(order.Id, reviewerId))
            throw new ConflictException("Bạn đã đánh giá buổi hẹn này rồi.");

        var review = new DateReview
        {
            Id = Guid.NewGuid(),
            DatePassOrderId = order.Id,
            MatchId = order.MatchId,
            ReviewerId = reviewerId,
            RevieweeId = revieweeId,
            Rating = dto.Rating,
            Comment = string.IsNullOrWhiteSpace(dto.Comment) ? null : dto.Comment.Trim(),
            CreatedAt = DateTime.UtcNow,
        };
        await _reviewRepository.AddAsync(review);
        await _reviewRepository.SaveChangesAsync();

        return ToDto(review);
    }

    public async Task<List<PendingReviewDto>> GetPendingAsync(Guid userId)
    {
        var matches = await _matchService.GetMyMatchesAsync(userId);
        if (matches.Count == 0) return new();

        var partnerByMatch = matches.ToDictionary(m => m.MatchId, m => m);
        var orders = await _orderRepository.GetForMatchesAsync(partnerByMatch.Keys);
        var redeemed = orders.Where(o => o.Status == DatePassStatus.Redeemed).ToList();
        if (redeemed.Count == 0) return new();

        var reviewed = await _reviewRepository.GetReviewedOrderIdsAsync(userId, redeemed.Select(o => o.Id));

        var result = new List<PendingReviewDto>();
        foreach (var o in redeemed)
        {
            if (reviewed.Contains(o.Id)) continue;
            if (!partnerByMatch.TryGetValue(o.MatchId, out var partner)) continue; // chỉ buổi hẹn thuộc match của user
            result.Add(new PendingReviewDto
            {
                DatePassOrderId = o.Id,
                PartnerId = partner.UserId,
                PartnerName = partner.DisplayName,
                PartnerAvatarUrl = partner.AvatarUrl,
                VenueName = o.VenueName,
                ComboTitle = o.ComboTitle,
                RedeemedAt = o.RedeemedAt,
            });
        }
        return result;
    }

    public async Task<ProfileReviewsDto> GetProfileReviewsAsync(Guid viewerId, Guid targetUserId)
    {
        var (count, avg) = await _reviewRepository.GetSummaryAsync(targetUserId);
        var dto = new ProfileReviewsDto { RatingCount = count, RatingAvg = Math.Round(avg, 1) };
        if (count == 0) return dto;

        var reviews = await _reviewRepository.GetForRevieweeAsync(targetUserId);

        // CHÍNH CHỦ xem hồ sơ mình → đọc được nội dung + LỘ danh tính người đã đánh giá.
        if (viewerId == targetUserId)
        {
            var reviewers = (await _userRepository.GetWithProfileByIdsAsync(reviews.Select(r => r.ReviewerId).Distinct()))
                .ToDictionary(u => u.Id, u => u.Profile);
            dto.Reviews = reviews.Select(r =>
            {
                reviewers.TryGetValue(r.ReviewerId, out var p);
                return ToDto(r, p?.DisplayName ?? "Người dùng", p?.AvatarUrl);
            }).ToList();
            return dto;
        }

        // NGƯỜI KHÁC xem → nội dung chỉ mở cho Gold, và luôn ẨN DANH người viết.
        var ent = await _subscriptionService.GetEntitlementsAsync(viewerId);
        if (!ent.CanSeeDateReviews)
        {
            dto.Locked = true;
            return dto;
        }
        dto.Reviews = reviews.Select(r => ToDto(r)).ToList();
        return dto;
    }

    /// <summary>reviewerName != null → lộ danh tính (chính chủ xem); null → ẩn danh (người khác xem).</summary>
    private static DateReviewDto ToDto(DateReview r, string? reviewerName = null, string? reviewerAvatarUrl = null)
    {
        var reveal = reviewerName != null;
        return new DateReviewDto
        {
            Id = r.Id,
            ReviewerId = reveal ? r.ReviewerId : Guid.Empty,
            ReviewerName = reveal ? reviewerName! : "Ẩn danh",
            ReviewerAvatarUrl = reveal ? reviewerAvatarUrl : null,
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt,
        };
    }
}
