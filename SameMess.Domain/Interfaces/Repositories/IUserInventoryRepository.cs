using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IUserInventoryRepository : IBaseRepository<UserInventory>
{
    Task<List<UserInventory>> GetByUserAsync(Guid userId);
    Task<UserInventory?> GetAsync(Guid userId, string materialType);
}
