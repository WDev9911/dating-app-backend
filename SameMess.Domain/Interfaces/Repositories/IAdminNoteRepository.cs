using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IAdminNoteRepository : IBaseRepository<AdminNote>
{
    Task<List<AdminNote>> GetByUserIdAsync(Guid userId);
}
