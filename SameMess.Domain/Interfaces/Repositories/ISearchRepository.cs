using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface ISearchRepository
{
    /// <summary>
    /// Tìm hồ sơ theo bộ lọc tường minh (khác Discovery dựa trên preferences). Lọc thô ở DB:
    /// hồ sơ đã hoàn thiện, loại bản thân + người bị loại, giới tính, khoảng tuổi (ngày sinh),
    /// thành phố (Location), và (nếu có) chỉ người mang ít nhất một trong các sở thích chỉ định.
    /// Lọc tinh khoảng cách + sắp xếp làm ở tầng service.
    /// </summary>
    Task<List<UserProfile>> SearchAsync(
        Guid excludeUserId,
        IReadOnlyCollection<Guid> excludeUserIds,
        string? gender,
        string? city,
        DateOnly? minBirthDate,
        DateOnly? maxBirthDate,
        IReadOnlyCollection<Guid> interestIds,
        int fetchLimit);

    /// <summary>Danh sách thành phố (Location) phân biệt từ các hồ sơ đã hoàn thiện — cho bộ lọc.</summary>
    Task<List<string>> GetDistinctCitiesAsync(int limit);
}
