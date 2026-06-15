using SameMess.Application.DTOs.Events;

namespace SameMess.Application.Interfaces.Services;

public interface IEventService
{
    Task<List<EventDto>> GetEventsAsync(Guid userId);
    Task<EventDto> GetEventAsync(Guid userId, Guid eventId);
    Task<RegisterResultDto> RegisterAsync(Guid userId, Guid eventId);
    Task<List<EventHistoryDto>> GetHistoryAsync(Guid userId);
    Task<EventRewardDto> GetRewardAsync(Guid eventId);
}
