using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IPhotoRepository : IBaseRepository<Photo>
{
    Task<List<Photo>> GetByUserIdAsync(Guid userId);
}
