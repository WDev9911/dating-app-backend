using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IEventRepository : IBaseRepository<Event>
{
    Task<List<Event>> GetPublishedAsync();
}
