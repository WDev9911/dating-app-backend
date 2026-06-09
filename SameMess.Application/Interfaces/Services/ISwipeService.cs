using SameMess.Application.DTOs.Matching;

namespace SameMess.Application.Interfaces.Services;

public interface ISwipeService
{
    Task<SwipeResultDto> SwipeAsync(Guid userId, SwipeRequestDto dto);

    /// <summary>Những người đã Like (thường) tôi mà tôi chưa swipe lại (premium tiềm năng).</summary>
    Task<List<LikedMeProfileDto>> GetWhoLikedMeAsync(Guid userId);

    /// <summary>Những người đã SuperLike tôi mà tôi chưa swipe lại (mục riêng, xem miễn phí).</summary>
    Task<List<LikedMeProfileDto>> GetWhoSuperLikedMeAsync(Guid userId);

    /// <summary>Hoàn tác lần swipe gần nhất (tính năng premium tiềm năng).</summary>
    Task<UndoResultDto> UndoLastSwipeAsync(Guid userId);
}
