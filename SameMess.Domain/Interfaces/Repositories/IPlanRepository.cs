using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IPlanRepository : IBaseRepository<Plan>
{
    Task<List<Plan>> GetActiveAsync();
    Task<Plan?> GetByCodeAsync(string code);
}
