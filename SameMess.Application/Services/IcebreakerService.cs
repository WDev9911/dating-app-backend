using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Exceptions;
using SameMess.Domain.Interfaces.Repositories;

namespace SameMess.Application.Services;

public class IcebreakerService : IIcebreakerService
{
    private readonly IMatchRepository _matchRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAiAssistantService _aiAssistant;

    public IcebreakerService(
        IMatchRepository matchRepository,
        IUserRepository userRepository,
        IAiAssistantService aiAssistant)
    {
        _matchRepository = matchRepository;
        _userRepository = userRepository;
        _aiAssistant = aiAssistant;
    }

    public async Task<List<string>> SuggestForMatchAsync(Guid userId, Guid matchId)
    {
        var match = await _matchRepository.GetByIdForUserAsync(matchId, userId)
            ?? throw new NotFoundException("Match", matchId);

        if (!match.IsActive)
            throw new ForbiddenException("This match is no longer active.");

        var otherId = match.UserAId == userId ? match.UserBId : match.UserAId;

        var users = await _userRepository.GetWithProfileByIdsAsync(new[] { userId, otherId });
        var me = users.FirstOrDefault(u => u.Id == userId);
        var other = users.FirstOrDefault(u => u.Id == otherId);

        return await _aiAssistant.SuggestIcebreakersAsync(
            me?.Profile?.Bio,
            other?.Profile?.Bio,
            other?.Profile?.DisplayName);
    }
}
