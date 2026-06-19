using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IVenueRepository : IBaseRepository<Venue>
{
    Task<List<Venue>> GetActiveAsync(string? category);
    Task<List<Venue>> GetAllAsync(bool includeInactive);
}
