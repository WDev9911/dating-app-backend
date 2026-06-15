using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IPhotoRepository : IBaseRepository<Photo>
{
    Task<List<Photo>> GetByUserIdAsync(Guid userId);

    /// <summary>Ảnh theo trạng thái kiểm duyệt (kèm User+Profile để hiển thị chủ ảnh).</summary>
    Task<List<Photo>> GetByStatusAsync(string status);
}
