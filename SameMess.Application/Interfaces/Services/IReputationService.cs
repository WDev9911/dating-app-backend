using SameMess.Application.DTOs.Reputation;

namespace SameMess.Application.Interfaces.Services;

public interface IReputationService
{
    /// <summary>
    /// Ghi nhận một sự kiện uy tín (ReputationEventType) rồi tính lại điểm + tier.
    /// Gọi từ các service sẵn có (fail-safe). Sự kiện "1 lần" sẽ bỏ qua nếu đã có.
    /// </summary>
    Task RecordEventAsync(Guid userId, string eventType, string? reason = null);

    /// <summary>Chi tiết uy tín cho chính chủ (kèm số điểm + cách cải thiện).</summary>
    Task<ReputationDto> GetMyReputationAsync(Guid userId);

    /// <summary>Điểm hiện tại của nhiều user (cho Discovery xếp hạng). Thiếu thì coi như khởi điểm.</summary>
    Task<Dictionary<Guid, int>> GetScoresAsync(IEnumerable<Guid> userIds);
}
