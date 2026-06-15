using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IEventRegistrationRepository : IBaseRepository<EventRegistration>
{
    Task<EventRegistration?> GetAsync(Guid eventId, Guid userId);
    Task<int> CountByEventAsync(Guid eventId);
    Task<List<EventRegistration>> GetUserRegistrationsAsync(Guid userId);
    Task<List<Guid>> GetRegisteredEventIdsAsync(Guid userId);
    Task<Dictionary<Guid, int>> GetCountsByEventsAsync(IReadOnlyCollection<Guid> eventIds);
}
