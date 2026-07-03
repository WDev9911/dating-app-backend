using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByPhoneNumberAsync(string phoneNumber);
    Task<bool> EmailExistsAsync(string email);
    Task<bool> PhoneExistsAsync(string phoneNumber);
    Task<User?> GetWithProfileAsync(Guid userId);
    Task<User?> GetFullProfileAsync(Guid userId);
    Task<List<User>> GetWithProfileByIdsAsync(IEnumerable<Guid> ids);
    Task<List<User>> GetWithProfileAndPhotosByIdsAsync(IEnumerable<Guid> ids);
    Task<List<User>> GetAdminsWithProfileAsync();

    /// <summary>Hồ sơ đang chờ admin duyệt xác minh khuôn mặt (kèm Profile + Photos).</summary>
    Task<List<User>> GetPendingFaceVerificationsAsync();

    /// <summary>Xoá VĨNH VIỄN toàn bộ dữ liệu liên quan tới user (mọi schema) rồi xoá user.</summary>
    Task PurgeAsync(Guid userId);
}
