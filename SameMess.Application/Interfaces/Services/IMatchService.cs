using SameMess.Application.DTOs.Matching;

namespace SameMess.Application.Interfaces.Services;

public interface IMatchService
{
    Task<List<MatchDto>> GetMyMatchesAsync(Guid userId);
    Task UnmatchAsync(Guid userId, Guid matchId);
}
