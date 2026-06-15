using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IEmergencyContactRepository : IBaseRepository<EmergencyContact>
{
    Task<List<EmergencyContact>> GetByUserIdAsync(Guid userId);

    /// <summary>Thay toàn bộ liên hệ khẩn cấp của user bằng danh sách mới.</summary>
    Task ReplaceForUserAsync(Guid userId, IEnumerable<EmergencyContact> contacts);
}
