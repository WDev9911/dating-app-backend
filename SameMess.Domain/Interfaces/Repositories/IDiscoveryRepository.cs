using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IDiscoveryRepository
{
    /// <summary>
    /// Lọc thô ứng viên: loại bản thân, chỉ lấy profile đã hoàn thiện, có tọa độ nằm trong
    /// bounding box, đúng giới tính (null = mọi giới tính) và ngày sinh trong khoảng tuổi.
    /// Lọc 2 chiều: chỉ giữ người mà tôi (viewerGender/viewerAge) cũng nằm trong tiêu chí của họ.
    /// Lọc tinh (khoảng cách tròn, tuổi chính xác) thực hiện ở tầng service.
    /// </summary>
    Task<List<UserProfile>> GetCandidatesAsync(
        Guid excludeUserId,
        IReadOnlyCollection<Guid> excludeUserIds,
        string? requiredGender,
        string? viewerGender,
        int viewerAge,
        DateOnly minBirthDate,
        DateOnly maxBirthDate,
        double minLat,
        double maxLat,
        double minLon,
        double maxLon,
        int fetchLimit);
}
