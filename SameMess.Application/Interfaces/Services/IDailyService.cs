using SameMess.Application.DTOs.Daily;

namespace SameMess.Application.Interfaces.Services;

public interface IDailyService
{
    Task<DailyConnectionDto> GetConnectionAsync(Guid userId);
    Task<CompleteDailyResultDto> CompleteAsync(Guid userId, CompleteDailyDto dto);
}
